namespace finance_tracker.backend.DTOs.Shared
{
    public class ApiResponseDto<T>
    {
        public required bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public required List<string> Errors { get; set; } = new();
    }
}