using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs
{
    public class ApiResponseDto<T>
    {
        [Required]
        public bool Success { get; set; }
        
        public string? Message { get; set; }
        
        public T? Data { get; set; }
    }
}
