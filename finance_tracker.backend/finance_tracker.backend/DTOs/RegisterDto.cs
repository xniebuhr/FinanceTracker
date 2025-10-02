using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
