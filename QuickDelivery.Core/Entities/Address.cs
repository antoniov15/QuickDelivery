using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickDelivery.Core.Entities
{
    [Table("Addresses")]
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        [StringLength(255)]
        public string? Instructions { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        // Foreign Key relationships - simplified
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        // Computed property
        [NotMapped]
        public string FullAddress => $"{Street}, {City}, {PostalCode}, {Country}";
    }
}