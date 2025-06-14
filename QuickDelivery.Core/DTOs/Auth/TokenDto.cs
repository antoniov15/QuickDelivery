// Models/DTOs/Auth/TokenDto.cs
using QuickDelivery.Core.DTOs.Users;

namespace QuickDelivery.Core.DTOs.Auth
{
    public class TokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
    }
}

