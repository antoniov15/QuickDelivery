using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string? TransactionId { get; set; }

        [StringLength(255)]
        public string? PaymentGateway { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        // Foreign Key relationships
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}