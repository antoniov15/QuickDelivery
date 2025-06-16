using Microsoft.Extensions.Options;
using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Core.Interfaces.Services;
using QuickDelivery.Core.Options;
using System.Security.Cryptography;
using System.Text;

namespace QuickDelivery.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;

        public UserService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapUserToDto);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapUserToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapUserToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            return user != null ? MapUserToDto(user) : null;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByUsernameAsync(loginRequest.Username);

            if (user == null || !VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Numele de utilizator sau parola sunt incorecte");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException("Contul de utilizator este inactiv");
            }

            // Actualizăm data ultimei autentificări
            await _userRepository.UpdateLastLoginAsync(user.UserId);

            // Generăm token JWT
            var token = _tokenService.GenerateJwtToken(
                user.UserId,
                user.Email,
                new[] { user.Role.ToString() }
            );

            return new LoginResponseDto
            {
                Token = token,
                TokenExpiration = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                User = MapUserToDto(user)
            };
        }
        
        public async Task<UserDto> RegisterAsync(RegisterUserDto registerRequest)
        {
            // Verificăm dacă email-ul există deja
            if (await _userRepository.ExistsByEmailAsync(registerRequest.Email))
            {
                throw new InvalidOperationException("Email is already in use");
            }

            // Generăm un username din email dacă nu este furnizat
            string username = registerRequest.Email.Split('@')[0] + DateTime.UtcNow.Ticks % 1000;

            // Creăm utilizatorul nou
            var newUser = new User
            {
                Email = registerRequest.Email,
                Username = username, // Generăm un username unic
                PasswordHash = HashPassword(registerRequest.Password),
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                PhoneNumber = registerRequest.PhoneNumber,
                Role = registerRequest.Role ?? UserRole.Customer, // Folosim rolul specificat sau Customer implicit
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow,
                ProfileImageUrl = null
            };

            var createdUser = await _userRepository.CreateAsync(newUser);
            return MapUserToDto(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateRequest)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException("Utilizatorul nu a fost găsit");
            }

            // Verificăm unicitatea email-ului dacă acesta este modificat
            if (updateRequest.Email != null && updateRequest.Email != user.Email)
            {
                if (await _userRepository.ExistsByEmailAsync(updateRequest.Email))
                {
                    throw new InvalidOperationException("Adresa de email este deja utilizată");
                }
                user.Email = updateRequest.Email;
            }

            // Verificăm unicitatea username-ului dacă acesta este modificat
            if (updateRequest.Username != null && updateRequest.Username != user.Username)
            {
                if (await _userRepository.ExistsByUsernameAsync(updateRequest.Username))
                {
                    throw new InvalidOperationException("Numele de utilizator este deja utilizat");
                }
                user.Username = updateRequest.Username;
            }

            // Actualizăm celelalte câmpuri dacă sunt furnizate
            if (updateRequest.FirstName != null)
                user.FirstName = updateRequest.FirstName;

            if (updateRequest.LastName != null)
                user.LastName = updateRequest.LastName;

            if (updateRequest.PhoneNumber != null)
                user.PhoneNumber = updateRequest.PhoneNumber;

            if (updateRequest.IsActive.HasValue)
                user.IsActive = updateRequest.IsActive.Value;

            if (updateRequest.Role != null)
                user.Role = updateRequest.Role.Value;

            var updatedUser = await _userRepository.UpdateAsync(user);
            return MapUserToDto(updatedUser);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException("Utilizatorul nu a fost găsit");
            }

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new InvalidOperationException("Parola curentă este incorectă");
            }

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _userRepository.ExistsByEmailAsync(email);
        }

        public async Task<bool> UserExistsByUsernameAsync(string username)
        {
            return await _userRepository.ExistsByUsernameAsync(username);
        }

        #region Metode helper

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified,
                Role = user.Role,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt,
                CustomerId = user.CustomerId,
                PartnerId = user.Partner?.PartnerId
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convertim șirul în matrice de bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Calculăm hash-ul
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convertim matricea de bytes în șir
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            string hashedPassword = HashPassword(password);
            return string.Equals(hashedPassword, passwordHash, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}