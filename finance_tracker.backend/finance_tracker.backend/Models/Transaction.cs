namespace finance_tracker.backend.Models
{
	public class Transaction
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public string Category { get; set; } = string.Empty;
		public decimal Amount { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
