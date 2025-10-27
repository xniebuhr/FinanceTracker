using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Transactions
{
    public class UpdateTransactionRequestDto : CreateTransactionRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}