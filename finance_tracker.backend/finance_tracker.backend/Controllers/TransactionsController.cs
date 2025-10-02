using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

using finance_tracker.backend.Data;
using finance_tracker.backend.DTOs;
using finance_tracker.backend.Models;

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

        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: api/transactions?page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<PagedResultDto<TransactionDto>>>>
            GetAll(int page = 1, int pageSize = 20)
        {
            var query = _context.Transactions
                .Where(t => t.ApplicationUserId == CurrentUserId)
                .OrderByDescending(t => t.Date);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Type = t.Type,
                    Amount = t.Amount,
                    Date = t.Date,
                    Description = t.Description,
                    IsRecurring = t.IsRecurring,
                    Recurrence = t.Recurrence
                })
                .ToListAsync();

            var paged = new PagedResultDto<TransactionDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return Ok(new ApiResponseDto<PagedResultDto<TransactionDto>>
            {
                Success = true,
                Message = "Transactions retrieved successfully",
                Data = paged
            });
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<TransactionDto>>> GetById(int id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.ApplicationUserId == CurrentUserId)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found"
                });
            }

            var dto = new TransactionDto
            {
                Id = transaction.Id,
                Type = transaction.Type,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                IsRecurring = transaction.IsRecurring,
                Recurrence = transaction.Recurrence
            };

            return Ok(new ApiResponseDto<TransactionDto>
            {
                Success = true,
                Message = "Transaction retrieved successfully",
                Data = dto
            });
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<TransactionDto>>> Create(
            CreateTransactionDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input"
                });
            }

            if (input.IsRecurring && input.Recurrence == null)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Recurring transactions must specify a recurrence interval."
                });
            }

            var entity = new Transaction
            {
                ApplicationUserId = CurrentUserId,
                Type = input.Type,
                Amount = input.Amount,
                Date = input.Date,
                Description = input.Description,
                IsRecurring = input.IsRecurring,
                Recurrence = input.Recurrence
            };

            _context.Transactions.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new TransactionDto
            {
                Id = entity.Id,
                Type = entity.Type,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                IsRecurring = entity.IsRecurring,
                Recurrence = entity.Recurrence
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id },
                new ApiResponseDto<TransactionDto>
                {
                    Success = true,
                    Message = "Transaction created successfully",
                    Data = dto
                });
        }

        // PUT: api/transactions/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<TransactionDto>>> Update(
            int id, UpdateTransactionDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid input"
                });
            }

            if (id != input.Id)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "ID in URL and payload do not match."
                });
            }

            var entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == CurrentUserId);

            if (entity == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found"
                });
            }

            entity.Type = input.Type;
            entity.Amount = input.Amount;
            entity.Date = input.Date;
            entity.Description = input.Description;
            entity.IsRecurring = input.IsRecurring;
            entity.Recurrence = input.Recurrence;

            await _context.SaveChangesAsync();

            var dto = new TransactionDto
            {
                Id = entity.Id,
                Type = entity.Type,
                Amount = entity.Amount,
                Date = entity.Date,
                Description = entity.Description,
                IsRecurring = entity.IsRecurring,
                Recurrence = entity.Recurrence
            };

            return Ok(new ApiResponseDto<TransactionDto>
            {
                Success = true,
                Message = "Transaction updated successfully",
                Data = dto
            });
        }


        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<object>>> Delete(int id)
        {
            var entity = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == CurrentUserId);

            if (entity == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Transaction not found"
                });
            }

            _context.Transactions.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Transaction deleted successfully",
            });
        }
    }
}