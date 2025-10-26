using System.ComponentModel.DataAnnotations;

namespace finance_tracker.backend.DTOs.Shared
{
    public class PagedResultDto<T>
    {
        [Required]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        [Required]
        public int TotalCount { get; set; }

        [Required]
        public int Page { get; set; }

        [Required]
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}