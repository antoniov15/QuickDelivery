using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        // Proprietate calculată pentru numele complet
        public string FullName => $"{FirstName} {LastName}".Trim();

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; } = false;

        public UserRole Role { get; set; } = UserRole.Customer; // Default role

        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Redenumim LastLogin la LastLoginAt pentru consistență
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public int? PartnerId { get; set; }
        public virtual Partner? Partner { get; set; }

        public virtual ICollection<Order> OrdersAsCustomer { get; set; } = new List<Order>();
        public virtual ICollection<Delivery> DeliveriesAsDeliverer { get; set; } = new List<Delivery>();
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}