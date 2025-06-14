using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Entities
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        public int? PartnerId { get; set; }

        [Required]
        public int DeliveryAddressId { get; set; }

        public int? PickupAddressId { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SubTotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Tax { get; set; } = 0;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        public DateTime? EstimatedDeliveryTime { get; set; }

        public DateTime? ActualDeliveryTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign Key relationships
        [ForeignKey("CustomerId")]
        public virtual User Customer { get; set; } = null!;

        [ForeignKey("PartnerId")]
        public virtual Partner? Partner { get; set; }

        [ForeignKey("DeliveryAddressId")]
        public virtual Address DeliveryAddress { get; set; } = null!;

        [ForeignKey("PickupAddressId")]
        public virtual Address? PickupAddress { get; set; }

        // Navigation properties
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual Delivery? Delivery { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}