using Microsoft.Extensions.Logging;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Orders;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Exceptions;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Core.Interfaces.Services;

namespace QuickDelivery.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return orders.Select(MapOrderToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            return MapOrderToDto(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
            return orders.Select(MapOrderToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByPartnerIdAsync(int partnerId)
        {
            var orders = await _orderRepository.GetOrdersByPartnerIdAsync(partnerId);
            return orders.Select(MapOrderToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            return orders.Select(MapOrderToDto);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto)
        {
            // Validare elemente comandă
            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            {
                throw new BadRequestException("Comanda trebuie să conțină cel puțin un produs");
            }

            // Creează o nouă comandă
            var order = new Order
            {
                OrderNumber = await _orderRepository.GenerateOrderNumberAsync(),
                CustomerId = orderDto.CustomerId,
                PartnerId = orderDto.PartnerId,
                DeliveryAddressId = orderDto.DeliveryAddressId,
                PickupAddressId = orderDto.PickupAddressId,
                Status = OrderStatus.Pending,
                SubTotal = orderDto.SubTotal,
                DeliveryFee = orderDto.DeliveryFee,
                Tax = orderDto.Tax,
                Discount = orderDto.Discount,
                TotalAmount = orderDto.SubTotal + orderDto.DeliveryFee + orderDto.Tax - orderDto.Discount,
                Notes = orderDto.Notes,
                SpecialInstructions = orderDto.SpecialInstructions,
                EstimatedDeliveryTime = orderDto.EstimatedDeliveryTime,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            // Adaugă produsele la comandă
            foreach (var itemDto in orderDto.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new NotFoundException($"Produsul cu ID-ul {itemDto.ProductId} nu a fost găsit");
                }

                if (!product.IsAvailable || product.StockQuantity < itemDto.Quantity)
                {
                    throw new BadRequestException($"Produsul '{product.Name}' nu este disponibil sau nu are stoc suficient");
                }

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * itemDto.Quantity,
                    SpecialInstructions = itemDto.SpecialInstructions
                };

                order.OrderItems.Add(orderItem);

                // Actualizează stocul produsului
                product.StockQuantity -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            // Salvează comanda în baza de date
            var createdOrder = await _orderRepository.CreateAsync(order);

            _logger.LogInformation("Comandă nouă creată: {OrderNumber} pentru clientul {CustomerId}",
                createdOrder.OrderNumber, createdOrder.CustomerId);

            return MapOrderToDto(createdOrder);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto updateOrderDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            // Actualizează statusul și alte informații
            order.Status = updateOrderDto.Status;

            if (!string.IsNullOrWhiteSpace(updateOrderDto.Notes))
            {
                order.Notes = updateOrderDto.Notes;
            }

            if (updateOrderDto.EstimatedDeliveryTime.HasValue)
            {
                order.EstimatedDeliveryTime = updateOrderDto.EstimatedDeliveryTime;
            }

            if (updateOrderDto.ActualDeliveryTime.HasValue)
            {
                order.ActualDeliveryTime = updateOrderDto.ActualDeliveryTime;
            }

            order.UpdatedAt = DateTime.UtcNow;

            // Salvează modificările
            var updatedOrder = await _orderRepository.UpdateAsync(order);

            _logger.LogInformation("Status comandă actualizat: {OrderNumber} la status {Status}",
                order.OrderNumber, order.Status);

            return MapOrderToDto(updatedOrder);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return false;
            }

            // Doar comenzile în starea Pending sau Cancelled pot fi șterse
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
            {
                throw new BadRequestException("Doar comenzile în starea 'În așteptare' sau 'Anulate' pot fi șterse");
            }

            // Restaurează stocul produselor
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            return await _orderRepository.DeleteAsync(id);
        }

        // Helper pentru maparea Order -> OrderDto
        private OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                PartnerId = order.PartnerId,
                DeliveryAddressId = order.DeliveryAddressId,
                PickupAddressId = order.PickupAddressId,
                Status = order.Status,
                SubTotal = order.SubTotal,
                DeliveryFee = order.DeliveryFee,
                Tax = order.Tax,
                Discount = order.Discount,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                SpecialInstructions = order.SpecialInstructions,
                EstimatedDeliveryTime = order.EstimatedDeliveryTime,
                ActualDeliveryTime = order.ActualDeliveryTime,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,

                // Relații
                Customer = order.Customer != null ? new UserBasicDto
                {
                    UserId = order.Customer.UserId,
                    FullName = $"{order.Customer.FirstName} {order.Customer.LastName}",
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber ?? string.Empty
                } : null,

                Partner = order.Partner != null ? new PartnerBasicDto
                {
                    PartnerId = order.Partner.PartnerId,
                    BusinessName = order.Partner.BusinessName,
                    LogoUrl = order.Partner.LogoUrl
                } : null,

                DeliveryAddress = order.DeliveryAddress != null ? new AddressDto
                {
                    AddressId = order.DeliveryAddress.AddressId,
                    AddressLine1 = order.DeliveryAddress.FullAddress,
                    City = order.DeliveryAddress.City,
                    PostalCode = order.DeliveryAddress.PostalCode
                } : null,

                PickupAddress = order.PickupAddress != null ? new AddressDto
                {
                    AddressId = order.PickupAddress.AddressId,
                    AddressLine1 = order.PickupAddress.FullAddress,
                    City = order.PickupAddress.City,
                    PostalCode = order.PickupAddress.PostalCode
                } : null,

                OrderItems = order.OrderItems.Select(item => new OrderItemDetailDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    SpecialInstructions = item.SpecialInstructions,
                    Product = new ProductBasicDto
                    {
                        ProductId = item.Product.ProductId,
                        Name = item.Product.Name,
                        ImageUrl = item.Product.ImageUrl
                    }
                }).ToList(),

                Delivery = order.Delivery != null ? new DeliveryDto
                {
                    DeliveryId = order.Delivery.DeliveryId,
                    DelivererId = order.Delivery.DelivererId,
                    Status = order.Delivery.Status,
                    AssignedAt = order.Delivery.AssignedAt,
                    PickedUpAt = order.Delivery.PickedUpAt,
                    DeliveredAt = order.Delivery.DeliveredAt
                } : null,

                Payment = order.Payment != null ? new PaymentDto
                {
                    PaymentId = order.Payment.PaymentId,
                    Amount = order.Payment.Amount,
                    Method = (int)order.Payment.Method,
                    ProcessedAt = order.Payment.ProcessedAt
                } : null
            };
        }
    }
}