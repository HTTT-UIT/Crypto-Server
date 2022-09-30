using API.Common.Queries;
using System.Linq.Expressions;

namespace API.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
        {
            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageQuery pageQuery)
        {
            return query.Paginate(pageQuery.Page, pageQuery.PageSize);
        }

        public static IQueryable<T> OrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool desc)
            => desc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
    }
}