using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using finance_tracker.backend.Data;
using finance_tracker.backend.DTOs.Shared;
using finance_tracker.backend.DTOs.Transactions;
using finance_tracker.backend.Models.Transactions;

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

        // Helper: get current authenticated user's ID from JWT claims
        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // ============================
        // GET: api/transactions?page=1&pageSize=20
        // Returns a paginated list of the current user's transactions
        // ============================
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<PagedResultDto<TransactionResponseDto>>>> GetAll(
            int page = 1, int pageSize = 20)
        {
            // Validate pagination inputs
            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = new List<string>
                    {
                        "Page must be >= 1 and pageSize must be between 1 and 100"
                    }
                });
            }

            // Query current user's transactions ordered by date (newest first)
            var query = _context.Transactions
                .AsNoTracking()
                .Where(t => t.ApplicationUserId == CurrentUserId)
                .OrderByDescending(t => t.TransactionDate);

            // Compute total count then fetch page slice
            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    Type = t.Type,
                    Category = t.Category,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    IsRecurring = t.IsRecurring,
                    Recurrence = t.Recurrence
                })
                .ToListAsync();

            // Package results
            var paged = new PagedResultDto<TransactionResponseDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            // Return success response
            return Ok(new ApiResponseDto<PagedResultDto<TransactionResponseDto>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = paged
            });
        }

        // ============================
        // GET: api/transactions/{id}
        // Returns a single transaction owned by the current user
        // ============================
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<TransactionResponseDto>>> GetById(int id)
        {
            // Look up transaction by ID scoped to current user
            var transaction = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == CurrentUserId);

            // Transaction not found or not owned by user
            if (transaction == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found",
                    Errors = new List<string> { "No transaction exists for the current user with that ID" }
                });
            }

            // Return success response
            return Ok(new ApiResponseDto<TransactionResponseDto>
            {
                Success = true,
                Message = "Transaction retrieved successfully",
                Data = new TransactionResponseDto
                {
                    Id = transaction.Id,
                    Type = transaction.Type,
                    Category = transaction.Category,
                    Amount = transaction.Amount,
                    TransactionDate = transaction.TransactionDate,
                    Description = transaction.Description,
                    IsRecurring = transaction.IsRecurring,
                    Recurrence = transaction.Recurrence
                }
            });
        }

        // ============================
        // POST: api/transactions
        // Creates a new transaction for the current user
        // ============================
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<TransactionResponseDto>>> Create(
            [FromBody] CreateTransactionRequestDto input)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Create entity with ownership bound to current user
            var entity = new Transaction
            {
                ApplicationUserId = CurrentUserId,
                Type = input.Type,
                Category = input.Category,
                Amount = input.Amount,
                TransactionDate = input.TransactionDate,
                Description = input.Description,
                IsRecurring = input.IsRecurring,
                Recurrence = input.Recurrence
            };

            // Persist to database
            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();

            // Return created response with location header
            return CreatedAtAction(nameof(GetById), new { id = entity.Id },
                new ApiResponseDto<TransactionResponseDto>
                {
                    Success = true,
                    Message = "Transaction created successfully",
                    Data = new TransactionResponseDto
                    {
                        Id = entity.Id,
                        Type = entity.Type,
                        Category = entity.Category,
                        Amount = entity.Amount,
                        TransactionDate = entity.TransactionDate,
                        Description = entity.Description,
                        IsRecurring = entity.IsRecurring,
                        Recurrence = entity.Recurrence
                    }
                });
        }

        // ============================
        // PUT: api/transactions/{id}
        // Updates an existing transaction owned by the current user
        // ============================
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<TransactionResponseDto>>> Update(
            int id, [FromBody] UpdateTransactionRequestDto input)
        {
            // Validate input model
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList()
                });
            }

            // Validate URL vs payload ID
            if (id != input.Id)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input",
                    Errors = new List<string> { "ID in URL and payload do not match" }
                });
            }

            // Look up entity scoped to current user
            var entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == CurrentUserId);

            // Transaction not found or not owned by user
            if (entity == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found",
                    Errors = new List<string> { "No transaction exists for the current user with that ID" }
                });
            }

            // Apply updates
            entity.Type = input.Type;
            entity.Category = input.Category;
            entity.Amount = input.Amount;
            entity.TransactionDate = input.TransactionDate;
            entity.Description = input.Description;
            entity.IsRecurring = input.IsRecurring;
            entity.Recurrence = input.Recurrence;

            // Persist changes
            await _context.SaveChangesAsync();

            // Return success response
            return Ok(new ApiResponseDto<TransactionResponseDto>
            {
                Success = true,
                Message = "Transaction updated successfully",
                Data = new TransactionResponseDto
                {
                    Id = entity.Id,
                    Type = entity.Type,
                    Category = entity.Category,
                    Amount = entity.Amount,
                    TransactionDate = entity.TransactionDate,
                    Description = entity.Description,
                    IsRecurring = entity.IsRecurring,
                    Recurrence = entity.Recurrence
                }
            });
        }

        // ============================
        // DELETE: api/transactions/{id}
        // Deletes a transaction owned by the current user
        // ============================
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            // Look up entity scoped to current user
            var entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == CurrentUserId);

            // Transaction not found or not owned by user
            if (entity == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found",
                    Errors = new List<string> { "No transaction exists for the current user with that ID" }
                });
            }

            // Remove and persist
            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();

            // Return success response
            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Transaction deleted successfully"
            });
        }
    }
}
