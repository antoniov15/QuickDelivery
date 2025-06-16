using System.ComponentModel.DataAnnotations;

namespace QuickDelivery.Core.DTOs.Users
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "User name can not be empty")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password can not be empty")]
        public string Password { get; set; } = string.Empty;
    }
}