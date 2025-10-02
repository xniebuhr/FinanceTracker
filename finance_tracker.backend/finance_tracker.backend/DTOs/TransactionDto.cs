using System.ComponentModel.DataAnnotations;

using finance_tracker.backend.Models;

namespace finance_tracker.backend.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }           // "Income" | "Expense"

        [Required, MaxLength(100)]
        public string Category { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }           // ISO 8601 (e.g., "2025-09-30")
        
        public string? Description { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceInterval? Recurrence { get; set; }    // "Weekly" | "Monthly" | "Quarterly" | "Yearly" | null
    }
}