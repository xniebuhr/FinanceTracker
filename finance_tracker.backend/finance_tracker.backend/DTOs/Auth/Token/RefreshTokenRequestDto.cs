using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Token
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}