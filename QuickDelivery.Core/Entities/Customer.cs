using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Alte proprietăți relevante pentru un client
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        // Relația cu utilizatorul (one-to-one)
        public int? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}