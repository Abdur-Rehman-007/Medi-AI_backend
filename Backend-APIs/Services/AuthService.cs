using Backend_APIs.DTOs;
using Backend_APIs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Backend_APIs.Services
{
    public class AuthService : IAuthService
    {
        private readonly MediaidbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(MediaidbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                if (existingUser != null)
                {
                    return (false, "Email already registered");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Create new user
                var user = new User
                {
                    Email = registerDto.Email,
                    PasswordHash = passwordHash,
                    FullName = registerDto.FullName,
                    Role = registerDto.Role,
                    Department = registerDto.Department,
                    RegistrationNumber = registerDto.RegistrationNumber,
                    PhoneNumber = registerDto.PhoneNumber,
                    DateOfBirth = registerDto.DateOfBirth,
                    Gender = registerDto.Gender,
                    Address = registerDto.Address,
                    IsEmailVerified = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate OTP
                var otp = GenerateOtp();
                var otpRecord = new Emailverificationotp
                {
                    UserId = user.Id,
                    Otp = otp,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Emailverificationotps.Add(otpRecord);
                await _context.SaveChangesAsync();

                // Send OTP via email
                var emailSent = await _emailService.SendOtpEmailAsync(user.Email, user.FullName, otp);

                return (true, $"Registration successful! OTP sent to {registerDto.Email}. Please check your email (or console in development mode).");
            }
            catch (Exception ex)
            {
                return (false, $"Registration failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message, string? Token, UserDto? User)> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == verifyOtpDto.Email);
                if (user == null)
                {
                    return (false, "User not found", null, null);
                }

                var otpRecord = await _context.Emailverificationotps
                    .Where(o => o.UserId == user.Id && o.Otp == verifyOtpDto.Otp && o.IsUsed == false)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    return (false, "Invalid OTP", null, null);
                }

                if (otpRecord.ExpiresAt < DateTime.UtcNow)
                {
                    return (false, "OTP expired", null, null);
                }

                // Mark OTP as used
                otpRecord.IsUsed = true;
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var userDto = MapToUserDto(user);

                return (true, "Email verified successfully! Welcome to MediAI Healthcare.", token, userDto);
            }
            catch (Exception ex)
            {
                return (false, $"Verification failed: {ex.Message}", null, null);
            }
        }

        public async Task<(bool Success, string Message, string? Token, UserDto? User)> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                if (user == null)
                {
                    return (false, "Invalid email or password", null, null);
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return (false, "Invalid email or password", null, null);
                }

                if (user.IsEmailVerified == false)
                {
                    return (false, "Please verify your email first", null, null);
                }

                if (user.IsActive == false)
                {
                    return (false, "Account is deactivated", null, null);
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);
                var userDto = MapToUserDto(user);

                return (true, "Login successful", token, userDto);
            }
            catch (Exception ex)
            {
                return (false, $"Login failed: {ex.Message}", null, null);
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryInHours"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                Department = user.Department,
                RegistrationNumber = user.RegistrationNumber,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Address = user.Address,
                ProfileImageUrl = user.ProfileImageUrl,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
