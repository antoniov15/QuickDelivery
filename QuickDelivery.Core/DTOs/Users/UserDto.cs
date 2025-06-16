using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.DTOs.Users
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; } = string.Empty;  // Proprietate adăugată
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public UserRole Role { get; set; }
        public string? ProfileImageUrl { get; set; }  // Proprietate adăugată
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }  // Proprietate adăugată
        public DateTime? LastLoginAt { get; set; }  // Redenumit din LastLogin
        public int? CustomerId { get; set; }
        public int? PartnerId { get; set; }
    }
}