using System.Security.Claims;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, string email, IEnumerable<string> roles);
        ClaimsPrincipal? ValidateToken(string token);
    }
}