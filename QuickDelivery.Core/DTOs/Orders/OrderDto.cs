using QuickDelivery.Core.DTOs.Products;
using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.DTOs.Orders
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int? PartnerId { get; set; }
        public int DeliveryAddressId { get; set; }
        public int? PickupAddressId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserBasicDto? Customer { get; set; }
        public PartnerBasicDto? Partner { get; set; }
        public AddressDto? DeliveryAddress { get; set; }
        public AddressDto? PickupAddress { get; set; }
        public List<OrderItemDetailDto> OrderItems { get; set; } = new List<OrderItemDetailDto>();
        public DeliveryDto? Delivery { get; set; }
        public PaymentDto? Payment { get; set; }
    }

    public class UserBasicDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class PartnerBasicDto
    {
        public int PartnerId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }

    public class AddressDto
    {
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }

    public class OrderItemDetailDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstructions { get; set; }
        public ProductBasicDto Product { get; set; } = null!;
    }

    public class ProductBasicDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class DeliveryDto
    {
        public int DeliveryId { get; set; }
        public int? DelivererId { get; set; }
        public DeliveryStatus Status { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }

    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}