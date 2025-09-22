using finance_tracker.backend.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_tracker.backend.Data
{
	public class FinanceContext : DbContext
	{
		public FinanceContext(DbContextOptions<FinanceContext> options)
			: base(options)
		{
		}

		public DbSet<Transaction> Transactions { get; set; }
	}
}