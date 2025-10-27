using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Users
{
    public class UpdateInfoRequestDto
    {
        [MaxLength(30)]
        public string? Username { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Username) &&
                string.IsNullOrWhiteSpace(FirstName) &&
                string.IsNullOrWhiteSpace(LastName) &&
                string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult(
                    "At least one field must be provided to update user info.",
                    new[] { nameof(Username), nameof(FirstName), nameof(LastName), nameof(PhoneNumber) });
            }
        }
    }
}