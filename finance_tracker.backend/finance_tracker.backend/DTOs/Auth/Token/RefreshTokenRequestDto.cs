using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Token
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required string RefreshToken { get; set; }
    }
}