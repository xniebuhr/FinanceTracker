namespace finance_tracker.backend.DTOs.Shared
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}