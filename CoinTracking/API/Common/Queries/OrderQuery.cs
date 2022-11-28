using Microsoft.AspNetCore.Mvc;

namespace API.Common.Queries
{
    public abstract class OrderQuery : PageQuery
    {
        [FromQuery]
        public string? SortBy { get; set; }

        [FromQuery]
        public string? SortDir { get; set; }
    }
}