using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Password
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        public string ResetToken { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}