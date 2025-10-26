using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Users
{
    public class UserResponseDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
