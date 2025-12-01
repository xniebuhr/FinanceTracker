using finance_tracker.backend.DTOs.Shared;
using finance_tracker.backend.DTOs.Users;
using finance_tracker.backend.DTOs.Auth.Password;
using finance_tracker.backend.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finance_tracker.backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // ============================
        // GET: api/users/me
        // Returns the current user's profile
        // ============================
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Get user ID from claims
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

            // Return user profile
            return Ok(new ApiResponseDto<UserResponseDto>
            {
                Success = true,
                Message = "User retrieved successfully",
                Data = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            });
        }

        // ============================
        // PUT: api/users/update
        // Updates the current user's profile
        // ============================
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateInfoRequestDto dto)
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

            // Get user ID from claims
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



            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                // Check for duplicate username
                var existingUser = await _userManager.FindByNameAsync(dto.Username);

                // Send error if username is already in use
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Update failed",
                        Errors = new List<string> { "That username is already taken" }
                    });
                }

                user.UserName = dto.Username;
            }
            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;


            // Save changes
            var result = await _userManager.UpdateAsync(user);

            // Send error if saving failed
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Update failed",
                    Errors = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList()
                });
            }

            // Return updated profile
            return Ok(new ApiResponseDto<UserResponseDto>
            {
                Success = true,
                Message = "User updated successfully",
                Data = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                }
            });
        }

        // ============================
        // POST: api/users/change-password
        // Changes the current user's password
        // ============================
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
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

            // Get user ID from claims
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

            // Attempt to change password
            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            // Send error if password change failed
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Password change failed",
                    Errors = result.Errors.Select(e => $"{e.Code}: {e.Description}").ToList()
                });
            }

            // Return success response
            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Password changed successfully"
            });
        }
    }
}