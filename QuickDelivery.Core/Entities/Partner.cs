using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickDelivery.Core.Entities
{
    [Table("Partners")]
    public class Partner
    {
        [Key]
        public int PartnerId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string BusinessName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(255)]
        public string? LogoUrl { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public TimeSpan OpenTime { get; set; }

        [Required]
        public TimeSpan CloseTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal AverageRating { get; set; } = 0;

        public int TotalOrders { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Foreign Key relationships
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; } = null!;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}