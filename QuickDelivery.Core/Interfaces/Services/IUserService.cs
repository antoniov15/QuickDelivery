using QuickDelivery.Core.DTOs.Users;

namespace QuickDelivery.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto> RegisterAsync(RegisterUserDto registerRequest);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateRequest);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<bool> UserExistsByUsernameAsync(string username);
    }
}