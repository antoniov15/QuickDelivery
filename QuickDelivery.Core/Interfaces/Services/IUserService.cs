using QuickDelivery.Core.Entities;
using QuickDelivery.Core.DTOs.Auth;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(RegisterDto registerDto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
    }
}