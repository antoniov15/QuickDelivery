using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuickDelivery.Core.DTOs;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Orders;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Database;
using System.Security.Claims;

namespace QuickDelivery.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly ApplicationDbContext _context;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger, ApplicationDbContext context)
        {
            _orderService = orderService;
            _logger = logger;
            _context = context; // Assuming IOrderService has a Context property for DbContext
        }

        // GET: api/Orders
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders()
        {
            try
            {
                // Obține ID-ul utilizatorului
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Invalid user ID"));
                }

                // Verifică rolul utilizatorului
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("User not found"));
                }

                // Doar admin poate vedea toate comenzile
                if (user.Role == UserRole.Admin)
                {
                    var allOrders = await _orderService.GetOrdersAsync();
                    return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                        allOrders,
                        "All orders retrieved successfully"
                    ));
                }

                // Pentru utilizatorii non-admin, folosește logica de filtrare din my-orders
                return await GetMyOrders();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving orders"));
            }
        }

        // GET: api/Orders/my-orders
        [Authorize]
        [HttpGet("my-orders")]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetMyOrders()
        {
            try
            {
                // Obține ID-ul utilizatorului din token
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Invalid user ID"));
                }

                // Verifică rolul utilizatorului
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("User not found"));
                }

                IEnumerable<OrderDto> orders;

                // Determină rolul utilizatorului și returnează comenzile corespunzătoare
                switch (user.Role)
                {
                    case UserRole.Admin:
                        // Admin poate vedea toate comenzile
                        orders = await _orderService.GetOrdersAsync();
                        break;

                    case UserRole.Customer:
                        // Găsește customerID pentru utilizatorul curent
                        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
                        if (customer == null)
                        {
                            // Dacă nu există un Customer, returnează o listă goală în loc de eroare
                            orders = new List<OrderDto>();
                            _logger.LogWarning("No customer record found for user ID {UserId}, returning empty orders list", userId);
                        }
                        else
                        {
                            // Customer vede doar comenzile lui
                            orders = await _orderService.GetOrdersByCustomerIdAsync(customer.CustomerId);
                        }
                        break;

                    case UserRole.Partner:
                        // Găsește partnerID pentru utilizatorul curent
                        var partner = await _context.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
                        if (partner == null)
                        {
                            return BadRequest(ApiResponse<object>.ErrorResult("Partner not found for this user"));
                        }

                        // Partner vede doar comenzile restaurantului său
                        orders = await _orderService.GetOrdersByPartnerIdAsync(partner.PartnerId);
                        break;

                    case UserRole.Deliverer:
                        // Folosind datele de deliverer pentru filtrare
                        // Presupunând că există o relație între User și Delivery
                        var deliveries = await _context.Deliveries.Where(d => d.DelivererId == userId).ToListAsync();
                        var orderIds = deliveries.Select(d => d.OrderId).ToList();

                        // Obține comenzile pentru aceste ID-uri
                        var delivererOrders = new List<OrderDto>();
                        foreach (var orderId in orderIds)
                        {
                            var orderDto = await _orderService.GetOrderByIdAsync(orderId);
                            if (orderDto != null)
                            {
                                delivererOrders.Add(orderDto);
                            }
                        }
                        orders = delivererOrders;
                        break;

                    default:
                        return StatusCode(StatusCodes.Status403Forbidden, ApiResponse<object>.ErrorResult("User role not authorized"));
                }

                _logger.LogInformation("Retrieved orders for user ID {UserId}", userId);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for current user");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving orders"));
            }
        }

        // GET: api/Orders/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                // Obține ID-ul utilizatorului
                if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("Invalid user ID"));
                }

                // Verifică rolul utilizatorului
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return Unauthorized(ApiResponse<object>.ErrorResult("User not found"));
                }

                // Verifică dacă utilizatorul are dreptul să vadă această comandă
                bool hasAccess = false;

                switch (user.Role)
                {
                    case UserRole.Admin:
                        // Admin poate vedea orice comandă
                        hasAccess = true;
                        break;

                    case UserRole.Customer:
                        // Verifică dacă este comanda clientului
                        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
                        hasAccess = customer != null && order.CustomerId == customer.CustomerId;
                        break;

                    case UserRole.Partner:
                        // Verifică dacă este o comandă a restaurantului partenerului
                        var partner = await _context.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
                        hasAccess = partner != null && order.PartnerId == partner.PartnerId;
                        break;

                    case UserRole.Deliverer:
                        // Verifică dacă livratorul este atribuit acestei comenzi
                        // Presupunând că Delivery are un DelivererId care este UserId-ul livratorului
                        var delivery = await _context.Deliveries.FirstOrDefaultAsync(d => d.OrderId == id);
                        hasAccess = delivery != null && delivery.DelivererId == userId;
                        break;

                    default:
                        hasAccess = false;
                        break;
                }

                if (!hasAccess)
                {
                    return Forbid();
                }

                _logger.LogInformation("Retrieved order with ID {OrderId}", id);

                return Ok(ApiResponse<OrderDto>.SuccessResult(
                    order,
                    "Order retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order with ID {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving the order"));
            }
        }

        /// <summary>
        /// Get orders for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Orders for the customer</returns>
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);

                _logger.LogInformation("Retrieved {Count} orders for customer with ID {CustomerId}",
                    orders.Count(), customerId);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Customer orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for customer with ID {CustomerId}", customerId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving customer orders"));
            }
        }

        /// <summary>
        /// Get orders for a partner
        /// </summary>
        /// <param name="partnerId">Partner ID</param>
        /// <returns>Orders for the partner</returns>
        [HttpGet("partner/{partnerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByPartnerId(int partnerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByPartnerIdAsync(partnerId);

                _logger.LogInformation("Retrieved {Count} orders for partner with ID {PartnerId}",
                    orders.Count(), partnerId);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Partner orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders for partner with ID {PartnerId}", partnerId);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving partner orders"));
            }
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status">Order status</param>
        /// <returns>Orders with the specified status</returns>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrdersByStatus(int status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync((Core.Enums.OrderStatus)status);

                _logger.LogInformation("Retrieved {Count} orders with status {Status}",
                    orders.Count(), (Core.Enums.OrderStatus)status);

                return Ok(ApiResponse<IEnumerable<OrderDto>>.SuccessResult(
                    orders,
                    "Orders retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders with status {Status}", (Core.Enums.OrderStatus)status);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving orders"));
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="createOrderDto">Order data</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid order data", errors));
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto);

                _logger.LogInformation("Created order with number {OrderNumber} for customer {CustomerId}",
                    order.OrderNumber, order.CustomerId);

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId },
                    ApiResponse<OrderDto>.SuccessResult(
                        order,
                        "Order created successfully"
                    ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order: {@CreateOrderDto}", createOrderDto);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while creating the order"));
            }
        }

        /// <summary>
        /// Update an order's status
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="updateOrderDto">Updated data</param>
        /// <returns>Updated order</returns>
        [Authorize(Roles = "Admin,Manager")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateOrderDto)
        {
            try
            {
                // Validate model
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid order status update data", errors));
                }

                var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderDto);

                if (order == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                _logger.LogInformation("Updated status for order {OrderId} to {Status}",
                    id, updateOrderDto.Status);

                return Ok(ApiResponse<OrderDto>.SuccessResult(
                    order,
                    "Order status updated successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating order status for order {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating the order status"));
            }
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Operation status</returns>
        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);

                if (!result)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("Order not found"));
                }

                _logger.LogInformation("Deleted order with ID {OrderId}", id);

                return Ok(ApiResponse<object>.SuccessResult("Order deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order with ID {OrderId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while deleting the order"));
            }
        }
    }
}