using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Token
{
    public class RefreshTokenResponseDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
