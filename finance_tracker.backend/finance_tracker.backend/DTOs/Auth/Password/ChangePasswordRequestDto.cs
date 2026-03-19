using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Password
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmNewPassword { get; set; }
    }
}