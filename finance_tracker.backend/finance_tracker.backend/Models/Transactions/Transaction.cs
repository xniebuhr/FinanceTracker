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

        [Required]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
