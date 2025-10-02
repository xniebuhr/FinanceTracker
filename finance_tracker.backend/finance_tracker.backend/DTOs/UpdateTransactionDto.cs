using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs
{
    public class UpdateTransactionDto : CreateTransactionDto
    {
        [Required]
        public int Id { get; set; }
    }
}