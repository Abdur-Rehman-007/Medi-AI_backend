using Backend_APIs.DTOs;
using Backend_APIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message) = await _authService.RegisterAsync(registerDto);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        /// <summary>
        /// Verify OTP and complete registration
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, token, user) = await _authService.VerifyOtpAsync(verifyOtpDto);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new
            {
                message,
                token,
                user
            });
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (success, message, token, user) = await _authService.LoginAsync(loginDto);

            if (!success)
            {
                return BadRequest(new { message });
            }

            return Ok(new
            {
                message,
                token,
                user
            });
        }

        /// <summary>
        /// Get current authenticated user details
        /// </summary>
        [HttpGet("current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var userId = int.Parse(userIdClaim.Value);
            var user = await _authService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Test endpoint to verify API is running
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                message = "MediAI Backend API is running"
            });
        }
    }
}
