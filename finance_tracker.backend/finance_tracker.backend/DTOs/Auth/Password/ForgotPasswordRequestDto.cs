using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Password
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}