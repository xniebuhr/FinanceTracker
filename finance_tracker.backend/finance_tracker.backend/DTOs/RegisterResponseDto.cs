using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs
{
    public class RegisterResponseDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;
    }
}
