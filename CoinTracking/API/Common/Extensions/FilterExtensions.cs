using API.Infrastructure.Entities.Common;

namespace API.Common.Extensions
{
    public static class FilterExtensions
    {
        public static IQueryable<T> FilterDeleted<T>(
            this IQueryable<T> source) where T : ISoftEntity
        {
            return source.Where(x => !x.Deleted);
        }
    }
}
