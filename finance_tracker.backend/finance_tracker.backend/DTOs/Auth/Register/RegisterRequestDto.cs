using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Register
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(30)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public required string Email { get; set; }

        [Required]
        [MinLength(8)]
        public required string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}