using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Login
{
    public class LoginResponseDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}