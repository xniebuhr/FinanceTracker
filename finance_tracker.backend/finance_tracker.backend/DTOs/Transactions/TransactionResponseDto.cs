using finance_tracker.backend.Models.Transactions;

namespace finance_tracker.backend.DTOs.Transactions
{
    public class TransactionResponseDto
    {
        public required int Id { get; set; }

        public required TransactionType Type { get; set; }

        public required string Category { get; set; }

        public required decimal Amount { get; set; }

        public required DateTime TransactionDate { get; set; }

        public string? Description { get; set; }

        public required bool IsRecurring { get; set; }

        public RecurrenceInterval? Recurrence { get; set; }
    }
}