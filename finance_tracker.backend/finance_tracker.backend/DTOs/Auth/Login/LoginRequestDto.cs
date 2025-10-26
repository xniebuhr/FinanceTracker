using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Login
{
    public class LoginRequestDto
    {
        [MaxLength(30)]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}