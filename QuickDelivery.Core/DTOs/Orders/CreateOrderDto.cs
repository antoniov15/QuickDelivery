using QuickDelivery.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        public int? PartnerId { get; set; }

        [Required(ErrorMessage = "Delivery address ID is required")]
        public int DeliveryAddressId { get; set; }

        public int? PickupAddressId { get; set; }

        [Required(ErrorMessage = "Subtotal is required")]
        [Range(0, 100000, ErrorMessage = "Subtotal must be between 0 and 100,000")]
        public decimal SubTotal { get; set; }

        [Required(ErrorMessage = "Delivery fee is required")]
        [Range(0, 10000, ErrorMessage = "Delivery fee must be between 0 and 10,000")]
        public decimal DeliveryFee { get; set; }

        [Range(0, 10000, ErrorMessage = "Tax must be between 0 and 10,000")]
        public decimal Tax { get; set; } = 0;

        [Range(0, 100000, ErrorMessage = "Discount must be between 0 and 100,000")]
        public decimal Discount { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [StringLength(500, ErrorMessage = "Special instructions cannot exceed 500 characters")]
        public string? SpecialInstructions { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }

        [Required(ErrorMessage = "Order items are required")]
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Special instructions cannot exceed 255 characters")]
        public string? SpecialInstructions { get; set; }
    }
}