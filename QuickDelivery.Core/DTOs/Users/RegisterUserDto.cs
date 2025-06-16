using System.ComponentModel.DataAnnotations;
using QuickDelivery.Core.Enums;

namespace QuickDelivery.Core.DTOs.Users
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string? FirstName { get; set; }

        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public UserRole? Role { get; set; }
    }
}