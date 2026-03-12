namespace finance_tracker.backend.DTOs.Shared
{
    public class PagedResultDto<T>
    {
        public required IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public required int TotalCount { get; set; }

        public required int Page { get; set; }

        public required int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}