using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finance_tracker.backend.Models
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
		public DateTime Date { get; set; }

		[Required]
		public string Category { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[Required]
		public TransactionType Type { get; set; }

		public string? Description { get; set; }

		[Required]
		[ForeignKey("ApplicationUser")]
		public string ApplicationUserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }

		public bool IsRecurring { get; set; }

		public RecurrenceInterval? Recurrence { get; set; }
	}
}
