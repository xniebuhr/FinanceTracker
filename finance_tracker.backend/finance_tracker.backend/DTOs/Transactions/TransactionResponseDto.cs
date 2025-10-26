using System.ComponentModel.DataAnnotations;
using finance_tracker.backend.Models.Transactions;

namespace finance_tracker.backend.DTOs.Transactions
{
    public class TransactionResponseDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceInterval? Recurrence { get; set; }
    }
}