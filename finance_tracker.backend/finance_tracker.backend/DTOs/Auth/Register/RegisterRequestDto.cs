using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Register
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}