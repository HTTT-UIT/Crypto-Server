using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using API.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Coins.Queries
{
    public class ListFavourite
    {
        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly MasterContext _dbContext;

            public Handler(MasterContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _dbContext.Coins.AsNoTracking();

                var total = await query.CountAsync(cancellationToken);

                var user = await _dbContext.Users.Where(x => x.Id == request.UserId)
                    .FirstOrDefaultAsync();

                query = query.Where(x => x.Users.Any(x => x.Id == request.UserId));

                if (!string.IsNullOrEmpty(request.Filter))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Filter.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.SortDir == SortDirection.Descending
                        ? query.OrderByDescending(request.SortBy)
                        : query.OrderBy(request.SortBy);
                }

                var items = await query
                    .Paginate(request)
                    .ToListAsync(cancellationToken);

                return new Response
                {
                    Items = items,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalRow = total
                };
            }
        }

        public class Query : OrderQuery, IRequest<Response>
        {
            public string? Filter { get; set; }

            [Required]
            public Guid UserId { get; set; }
        }

        public class Response : PagedResult<CoinEntity>
        {
        }
    }
}