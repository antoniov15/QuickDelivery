using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickDelivery.Api.Helpers;
using QuickDelivery.Core.DTOs.Common;
using QuickDelivery.Core.DTOs.Users;
using QuickDelivery.Core.Interfaces.Services;

namespace QuickDelivery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register([FromBody] RegisterUserDto registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(ApiResponse<object>.ErrorResult("Invalid registration data", errors));
                }

                var newUser = await _userService.RegisterAsync(registerRequest);

                _logger.LogInformation("User registered successfully: {Email}", registerRequest.Email);

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId },
                    ApiResponse<UserDto>.SuccessResult(
                        newUser,
                        "User registered successfully"
                    ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed: {Error}", ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration: {Error}", ex.Message);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during registration"));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                var loginResponse = await _userService.LoginAsync(loginRequest);
                return Ok(ApiResponse<LoginResponseDto>.SuccessResult(loginResponse, "Login successful"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login: {Error}", ex.Message);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred during login"));
            }
        }

        // Pentru ruta GetUserById folosită în CreatedAtAction
        [Authorize]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
        {
            //var user = await _userService.GetUserByIdAsync(id);
            //if (user == null)
            //    return NotFound(ApiResponse<object>.ErrorResult("User not found"));

            //return Ok(ApiResponse<UserDto>.SuccessResult(user, "User found"));

            try
            {
                if(!this.IsCurrentUserAuthorizedForResource(id))
                {
                    return Forbid();
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResult("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResult(user, "User found"));
            }
            catch
            {
                _logger.LogError("Error occurred while retrieving user with ID {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResult("An error occurred while retrieving the user"));
            }
        }
    }
}