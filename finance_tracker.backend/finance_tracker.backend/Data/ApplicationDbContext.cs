using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using finance_tracker.backend.Models;

namespace finance_tracker.backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {}

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();

            builder.Entity<Transaction>()
                .Property(t => t.Recurrence)
                .HasConversion<string>();

            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18,4);

            builder.Entity<Transaction>()
                .HasOne(t => t.ApplicationUser)
                .WithMany()
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}