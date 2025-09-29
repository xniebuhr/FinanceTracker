using Microsoft.AspNetCore.Identity;

namespace finance_tracker.backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
