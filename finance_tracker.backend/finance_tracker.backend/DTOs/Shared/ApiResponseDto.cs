using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Shared
{
    public class ApiResponseDto<T>
    {
        [Required]
        public bool Success { get; set; }
        
        public string? Message { get; set; }
        
        public T? Data { get; set; }

        [Required]
        public List<string> Errors { get; set; } = new();
    }
}