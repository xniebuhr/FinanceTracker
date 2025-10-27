using System.ComponentModel.DataAnnotations;
using finance_tracker.backend.Models.Transactions;

namespace finance_tracker.backend.DTOs.Transactions
{
    public class CreateTransactionRequestDto : IValidatableObject
    {
        [Required]
        public TransactionType Type { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsRecurring { get; set; }

        public RecurrenceInterval? Recurrence { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsRecurring && Recurrence == null)
            {
                yield return new ValidationResult(
                    "Recurrence interval is required when IsRecurring is true.",
                    new[] { nameof(Recurrence) });
            }
            if (!IsRecurring && Recurrence != null)
            {
                yield return new ValidationResult(
                    "Recurrence interval must be null when IsRecurring is false.",
                    new[] { nameof(Recurrence) });
            }
        }
    }
}