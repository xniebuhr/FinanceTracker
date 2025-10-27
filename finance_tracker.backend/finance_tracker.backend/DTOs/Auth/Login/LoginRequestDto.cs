using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Auth.Login
{
    public class LoginRequestDto : IValidatableObject
    {
        [MaxLength(30)]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "Either Username or Email must be provided.",
                    new[] { nameof(Username), nameof(Email) });
            }
        }
    }
}