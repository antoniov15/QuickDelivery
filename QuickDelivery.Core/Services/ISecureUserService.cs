using QuickDelivery.Core.DTOs.Users;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface ISecureUserService : IUserService
    {
        Task<UserDto?> GetUserByIdSecureAsync(int id, int requestingUserId, string requestingUserRole);
        Task<IEnumerable<UserDto>> GetAllUsersSecureAsync(int requestingUserId, string requestingUserRole);
    }
}