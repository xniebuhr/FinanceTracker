using finance_tracker.backend.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finance_tracker.backend.Models.Transactions
{
    public enum TransactionType
    {
        Income,
        Expense
    }

    public enum RecurrenceInterval
    {
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }

    public class Transaction
    {
        public int Id { get; set; }

        public TransactionType Type { get; set; }

        [Required, MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsRecurring { get; set; }

        public RecurrenceInterval? Recurrence { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; } = string.Empty;

        public ApplicationUser? ApplicationUser { get; set; }
    }
}
