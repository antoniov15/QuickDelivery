using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.Enums;
using QuickDelivery.Core.Interfaces.Services;
using System.Security.Claims;

namespace QuickDelivery.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        #region Admin Operations

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <returns>List of all users</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                _logger.LogInformation("Admin {AdminId} retrieved {Count} users",
                    GetCurrentUserId(), users.Count());

                return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResult(
                    users,
                    "Users retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all users");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving users"));
            }
        }

        /// <summary>
        /// Get a specific user by ID (Admin or own profile)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User with the specified ID</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // Check authorization: Admin can see all, users can only see their own profile
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid();
                }

                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                _logger.LogInformation("User {RequesterId} retrieved profile for user {UserId}",
                    currentUserId, id);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    user,
                    "User retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving the user"));
            }
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        /// <returns>Current user's profile</returns>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(currentUserId);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                _logger.LogInformation("User {UserId} retrieved own profile", currentUserId);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    user,
                    "Profile retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user profile");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving your profile"));
            }
        }

        /// <summary>
        /// Search users by email (Admin only)
        /// </summary>
        /// <param name="email">Email to search for</param>
        /// <returns>User with the specified email</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                _logger.LogInformation("Admin {AdminId} searched for user by email {Email}",
                    GetCurrentUserId(), email);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    user,
                    "User found successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching user by email {Email}", email);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while searching for the user"));
            }
        }

        /// <summary>
        /// Search users by username (Admin only)
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User with the specified username</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("by-username/{username}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);

                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                _logger.LogInformation("Admin {AdminId} searched for user by username {Username}",
                    GetCurrentUserId(), username);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    user,
                    "User found successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching user by username {Username}", username);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while searching for the user"));
            }
        }

        #endregion

        #region User Management Operations

        /// <summary>
        /// Update user profile (Admin can update any user, users can update their own profile)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // Check authorization
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid();
                }

                // Non-admin users cannot change certain fields
                if (currentUserRole != "Admin")
                {
                    updateUserDto.Role = null; // Users cannot change their own role
                    updateUserDto.IsActive = null; // Users cannot change their active status
                    updateUserDto.IsEmailVerified = null; // Users cannot change email verification status
                }

                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid user data", errors));
                }

                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

                _logger.LogInformation("User {RequesterId} updated profile for user {UserId}",
                    currentUserId, id);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    updatedUser,
                    "User updated successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update failed for user {UserId}: {Error}", id, ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating the user"));
            }
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        /// <param name="updateUserDto">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut("me")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Users cannot change certain fields about themselves
                updateUserDto.Role = null;
                updateUserDto.IsActive = null;
                updateUserDto.IsEmailVerified = null;

                return await UpdateUser(currentUserId, updateUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating current user profile");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating your profile"));
            }
        }

        /// <summary>
        /// Change password (Users can change their own password)
        /// </summary>
        /// <param name="changePasswordDto">Password change data</param>
        /// <returns>Success status</returns>
        [HttpPost("me/change-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid password data", errors));
                }

                var currentUserId = GetCurrentUserId();
                var success = await _userService.ChangePasswordAsync(currentUserId, changePasswordDto);

                if (success)
                {
                    _logger.LogInformation("User {UserId} changed password successfully", currentUserId);
                    return Ok(ApiResponse<object>.SuccessResult("Password changed successfully"));
                }

                return BadRequest(ApiResponse<object>.ErrorResult("Failed to change password"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Password change failed for user {UserId}: {Error}",
                    GetCurrentUserId(), ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user {UserId}",
                    GetCurrentUserId());
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while changing password"));
            }
        }

        #endregion

        #region Admin-Only Operations

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        /// <param name="registerUserDto">User registration data</param>
        /// <returns>Created user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                // Validate model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid user data", errors));
                }

                var newUser = await _userService.RegisterAsync(registerUserDto);

                _logger.LogInformation("Admin {AdminId} created new user {UserId} with email {Email}",
                    GetCurrentUserId(), newUser.UserId, newUser.Email);

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId },
                    ApiResponse<UserDto>.SuccessResult(
                        newUser,
                        "User created successfully"
                    ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User creation failed: {Error}", ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while creating the user"));
            }
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Operation status</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Prevent admin from deleting themselves
                if (currentUserId == id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("You cannot delete your own account"));
                }

                var success = await _userService.DeleteUserAsync(id);

                if (!success)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                _logger.LogInformation("Admin {AdminId} deleted user {UserId}", currentUserId, id);

                return Ok(ApiResponse<object>.SuccessResult("User deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while deleting the user"));
            }
        }

        /// <summary>
        /// Activate/Deactivate a user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="isActive">Active status</param>
        /// <returns>Updated user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUserStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Prevent admin from deactivating themselves
                if (currentUserId == id && !isActive)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("You cannot deactivate your own account"));
                }

                var updateDto = new UpdateUserDto { IsActive = isActive };
                var updatedUser = await _userService.UpdateUserAsync(id, updateDto);

                _logger.LogInformation("Admin {AdminId} {Action} user {UserId}",
                    currentUserId, isActive ? "activated" : "deactivated", id);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    updatedUser,
                    $"User {(isActive ? "activated" : "deactivated")} successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Status update failed for user {UserId}: {Error}", id, ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating user status"));
            }
        }

        /// <summary>
        /// Change user role (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="role">New role</param>
        /// <returns>Updated user</returns>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/role")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUserRole(int id, [FromBody] UserRole role)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Prevent admin from changing their own role
                if (currentUserId == id)
                {
                    return BadRequest(ApiResponse<object>.ErrorResult("You cannot change your own role"));
                }

                var updateDto = new UpdateUserDto { Role = role };
                var updatedUser = await _userService.UpdateUserAsync(id, updateDto);

                _logger.LogInformation("Admin {AdminId} changed role for user {UserId} to {Role}",
                    currentUserId, id, role);

                return Ok(ApiResponse<UserDto>.SuccessResult(
                    updatedUser,
                    $"User role updated to {role} successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Role update failed for user {UserId}: {Error}", id, ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role for user {UserId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while updating user role"));
            }
        }

        #endregion

        #region Utility Operations

        /// <summary>
        /// Check if email exists
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>Whether email exists</returns>
        [AllowAnonymous]
        [HttpGet("check-email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists(string email)
        {
            try
            {
                var exists = await _userService.UserExistsByEmailAsync(email);
                return Ok(ApiResponse<bool>.SuccessResult(exists, "Email availability checked"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking email {Email}", email);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while checking email"));
            }
        }

        /// <summary>
        /// Check if username exists
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Whether username exists</returns>
        [AllowAnonymous]
        [HttpGet("check-username/{username}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUsernameExists(string username)
        {
            try
            {
                var exists = await _userService.UserExistsByUsernameAsync(username);
                return Ok(ApiResponse<bool>.SuccessResult(exists, "Username availability checked"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking username {Username}", username);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while checking username"));
            }
        }

        /// <summary>
        /// Get user statistics (Admin only)
        /// </summary>
        /// <returns>User statistics</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> GetUserStatistics()
        {
            try
            {
                var allUsers = await _userService.GetAllUsersAsync();

                var stats = new
                {
                    TotalUsers = allUsers.Count(),
                    ActiveUsers = allUsers.Count(u => u.IsActive),
                    InactiveUsers = allUsers.Count(u => !u.IsActive),
                    VerifiedUsers = allUsers.Count(u => u.IsEmailVerified),
                    UnverifiedUsers = allUsers.Count(u => !u.IsEmailVerified),
                    UsersByRole = allUsers.GroupBy(u => u.Role)
                        .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                    RecentUsers = allUsers.Where(u => u.CreatedAt > DateTime.UtcNow.AddDays(-30)).Count(),
                    LastWeekLogins = allUsers.Where(u => u.LastLoginAt > DateTime.UtcNow.AddDays(-7)).Count()
                };

                _logger.LogInformation("Admin {AdminId} retrieved user statistics", GetCurrentUserId());

                return Ok(ApiResponse<object>.SuccessResult(stats, "User statistics retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving statistics"));
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        /// <returns>Current user ID</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        /// <summary>
        /// Get current user role from JWT token
        /// </summary>
        /// <returns>Current user role</returns>
        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        #endregion
    }
}