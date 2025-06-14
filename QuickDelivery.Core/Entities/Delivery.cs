using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Entities
{
    [Table("Deliveries")]
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public int? DelivererId { get; set; }

        [Required]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Available;

        public DateTime? AssignedAt { get; set; }

        public DateTime? PickedUpAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? DelivererRating { get; set; }

        [StringLength(500)]
        public string? DelivererReview { get; set; }

        [StringLength(500)]
        public string? DeliveryNotes { get; set; }

        // Current location tracking
        [Column(TypeName = "decimal(10,8)")]
        public decimal? CurrentLatitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? CurrentLongitude { get; set; }

        public DateTime? LastLocationUpdate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key relationships
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("DelivererId")]
        public virtual User? Deliverer { get; set; }
    }
}