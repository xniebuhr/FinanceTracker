using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Register
{
    public class RegisterResponseDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}