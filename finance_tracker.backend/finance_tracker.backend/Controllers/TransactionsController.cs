using finance_tracker.backend.Data;
using finance_tracker.backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace finance_tracker.backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return await _context.Transactions
                .Where(t => t.ApplicationUserId == userId)
                .ToListAsync();

        }

        // GET: api/transactions/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            transaction.ApplicationUserId = userId;

            if (transaction.IsRecurring && transaction.Recurrence == null)
            {
                return BadRequest("Recurring transactions must specify a recurrence interval.");
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        // PUT: api/transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, Transaction updatedTransaction)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != updatedTransaction.Id)
            {
                return BadRequest();
            }

            var existingTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == userId);

            if (existingTransaction == null)
            {
                return NotFound();
            }

            // Update fields
            existingTransaction.Type = updatedTransaction.Type;
            existingTransaction.Amount = updatedTransaction.Amount;
            existingTransaction.Date = updatedTransaction.Date;
            existingTransaction.Description = updatedTransaction.Description;
            existingTransaction.IsRecurring = updatedTransaction.IsRecurring;
            existingTransaction.Recurrence = updatedTransaction.Recurrence;

            await _context.SaveChangesAsync();
            return NoContent(); // standard for successful PUT

        }

        // DELETE: api/transactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == userId);

            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent(); // standard for successful DELETE
        }
    }
}
