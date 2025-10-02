using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace finance_tracker.backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}
