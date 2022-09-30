using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Common.Queries
{
    public abstract class PageQuery
    {
        [FromQuery]
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [FromQuery]
        [Range(0, int.MaxValue)]
        public int PageSize { get; set; } = 20;
    }
}