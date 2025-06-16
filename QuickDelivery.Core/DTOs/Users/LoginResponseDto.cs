namespace QuickDelivery.Core.DTOs.Users
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public UserDto User { get; set; } = new UserDto();
    }
}