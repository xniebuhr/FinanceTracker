using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs
{
    public class LoginResponseDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}
