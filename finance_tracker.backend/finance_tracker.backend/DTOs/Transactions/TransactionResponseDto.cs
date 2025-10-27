using finance_tracker.backend.Models.Transactions;

namespace finance_tracker.backend.DTOs.Transactions
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }

        public TransactionType Type { get; set; }

        public string Category { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public RecurrenceInterval? Recurrence { get; set; }
    }
}