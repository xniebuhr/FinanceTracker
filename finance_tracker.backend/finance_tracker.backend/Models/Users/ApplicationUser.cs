using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.Models.Users
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        public string? LastName { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiresAt { get; set; }

    }
}