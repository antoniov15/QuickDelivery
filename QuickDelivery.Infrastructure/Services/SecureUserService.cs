using Microsoft.Extensions.Options;
using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Core.Options;
using QuickDelivery.Infrastructure.Filters;
using QuickDelivery.Infrastructure.Repositories;

namespace QuickDelivery.Infrastructure.Services
{
    public class SecureUserService : UserService, ISecureUserService
    {
        private readonly RoleBasedDataFilter _dataFilter;

        public SecureUserService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions,
            RoleBasedDataFilter dataFilter)
            : base(userRepository, tokenService, jwtOptions)
        {
            _dataFilter = dataFilter;
        }

        public async Task<UserDto?> GetUserByIdSecureAsync(int id, int requestingUserId, string requestingUserRole)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var userDto = MapUserToDto(user);
            return _dataFilter.FilterUserData(userDto, requestingUserRole, requestingUserId);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersSecureAsync(int requestingUserId, string requestingUserRole)
        {
            // Doar Admin și Manager pot vedea toți utilizatorii
            if (requestingUserRole != "Admin" && requestingUserRole != "Manager")
            {
                throw new UnauthorizedAccessException("Access denied");
            }

            var users = await _userRepository.GetAllAsync();
            var userDtos = users.Select(MapUserToDto);

            return userDtos.Select(dto => _dataFilter.FilterUserData(dto, requestingUserRole, requestingUserId));
        }
    }
}