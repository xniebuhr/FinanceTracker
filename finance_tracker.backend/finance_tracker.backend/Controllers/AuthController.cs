using finance_tracker.backend.DTOs.Auth.Login;
using finance_tracker.backend.DTOs.Auth.Register;
using finance_tracker.backend.DTOs.Auth.Password;
using finance_tracker.backend.DTOs.Auth.Token;
using finance_tracker.backend.DTOs.Shared;
using finance_tracker.backend.Models.Users;
using finance_tracker.backend.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finance_tracker.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        // ============================
        // POST: api/auth/register
        // Handles user registration
        // ============================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Check for duplicate email
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = new List<string> { "An account with this email already exists" }
                });
            }

            // Check for duplicate username
            if (await _userManager.FindByNameAsync(dto.Username) != null)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = new List<string> { "An account with this username already exists" }
                });
            }

            // Create the new user
            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            // Save user to the database
            var result = await _userManager.CreateAsync(user, dto.Password);

            // Send error if the entry failed
            if (!result.Succeeded)
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = result.Errors
                                   .Select(e => $"{e.Code}: {e.Description}")
                                   .ToList()
                });

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshExpiry = _jwtTokenService.GetRefreshTokenExpiry();

            // Save refresh token to user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = refreshExpiry;
            await _userManager.UpdateAsync(user);

            // Return success response
            return Ok(new ApiResponseDto<RegisterResponseDto>
            {
                Success = true,
                Message = "User registered successfully",
                Data = new RegisterResponseDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    AccessToken = accessToken.AccessToken,
                    ExpiresAt = accessToken.ExpiresAt,
                    RefreshToken = refreshToken
                }
            });
        }

        // ============================
        // POST: api/auth/login
        // Handles user login by username or email
        // ============================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }
            
            // Find user by email or username
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(dto.Email))
                user = await _userManager.FindByEmailAsync(dto.Email);
            else if (!string.IsNullOrWhiteSpace(dto.Username))
                user = await _userManager.FindByNameAsync(dto.Username);

            // User not found
            if (user == null)
            {
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });
            }
            
            // Check if the password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            
            // Send error if login fails
            if (!result.Succeeded)
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid credentials"
                });

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshExpiry = _jwtTokenService.GetRefreshTokenExpiry();

            // Save refresh token to user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiresAt = refreshExpiry;
            await _userManager.UpdateAsync(user);

            // Return success response
            return Ok(new ApiResponseDto<LoginResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    AccessToken = accessToken.AccessToken,
                    ExpiresAt = accessToken.ExpiresAt,
                    RefreshToken = refreshToken
                }
            });
        }

        // ============================
        // POST: api/auth/forgot-password
        // Sends a reset token to the user's email
        // ============================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // If user exists, generate reset token
            if (user != null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                // TODO: Send resetToken via email (do not return it in API)
            }

            // Return success to prevent user enumeration
            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Password reset instructions have been sent to your email"
            });
        }

        // ============================
        // POST: api/auth/reset-password
        // Resets the user's password using a reset token
        // ============================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // Return success to prevent enumeration
            if (user == null)
            {
                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Password has been reset successfully"
                });
            }

            // Attempt to reset password
            var resetResult = await _userManager.ResetPasswordAsync(user, dto.ResetToken, dto.NewPassword);
            
            // Send error if reset fails
            if (!resetResult.Succeeded)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Password reset failed",
                    Errors = resetResult.Errors.Select(e => $"{e.Code}: {e.Description}").ToList()
                });
            }

            // Return success response
            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Password has been reset successfully"
            });
        }

        // ============================
        // POST: api/auth/refresh
        // Refreshes the access token using a valid refresh token
        // ============================
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Find user by ID
            var user = await _userManager.FindByIdAsync(dto.UserId);
            
            // User not found
            if (user == null)
            {
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid refresh request",
                    Errors = new List<string> { "User not found" }
                });
            }

            // Validate stored refresh token and expiry
            if (string.IsNullOrWhiteSpace(user.RefreshToken) ||
                user.RefreshToken != dto.RefreshToken ||
                user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                return Unauthorized(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid refresh request",
                    Errors = new List<string> { "Refresh token is invalid or expired" }
                });
            }

            // Generate new access token and rotate refresh token
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var newRefreshExpiry = _jwtTokenService.GetRefreshTokenExpiry();

            // Save new refresh token to user
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiresAt = newRefreshExpiry;
            await _userManager.UpdateAsync(user);

            // Return refreshed tokens
            return Ok(new ApiResponseDto<RefreshTokenResponseDto>
            {
                Success = true,
                Message = "Token refreshed successfully",
                Data = new RefreshTokenResponseDto
                {
                    AccessToken = newAccessToken.AccessToken,
                    ExpiresAt = newAccessToken.ExpiresAt,
                    RefreshToken = newRefreshToken
                }
            });
        }

        // ============================
        // DELETE: api/auth/delete
        // Deletes the currently authenticated user's account
        // ============================
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            // Get current user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Look up user in database
            var user = await _userManager.FindByIdAsync(userId);
            
            // User not found
            if (user == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "User not found",
                    Errors = new List<string> { "No account exists for the current user" }
                });
            }

            // Attempt to delete user
            var result = await _userManager.DeleteAsync(user);
            
            // If deletion fails, send error response
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Account deletion failed",
                    Errors = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList()
                });
            }

            // Return success response
            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Account deleted successfully"
            });
        }
    }
}