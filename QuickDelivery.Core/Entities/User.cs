using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        // Proprietate calculată pentru numele complet
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;

        public UserRole Role { get; set; } = UserRole.Customer; // Default role

        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public virtual Customer? Customer { get; set; }
        public virtual Partner? Partner { get; set; }

        // Computed properties pentru ID-urile relațiilor
        [NotMapped]
        public int? CustomerId => Customer?.CustomerId;

        [NotMapped]
        public int? PartnerId => Partner?.PartnerId;

        public virtual ICollection<Order> OrdersAsCustomer { get; set; } = new List<Order>();
        public virtual ICollection<Delivery> DeliveriesAsDeliverer { get; set; } = new List<Delivery>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}