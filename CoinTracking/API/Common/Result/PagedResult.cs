namespace API.Common.Result
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalRow { get; set; }
    }
}