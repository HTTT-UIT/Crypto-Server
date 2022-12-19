using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Coins.Queries
{
    public class List
    {
        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly MasterContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MasterContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _dbContext.Coins.AsNoTracking();

                var total = await query.CountAsync(cancellationToken);

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
                    .Include(x => x.Users)
                    .Paginate(request)
                    .ToListAsync(cancellationToken);

                var responseItems = _mapper.Map<List<Coin>>(items);

                return new Response
                {
                    Items = responseItems,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalRow = total
                };
            }
        }

        public class Query : OrderQuery, IRequest<Response>
        {
            public string? Filter { get; set; }
        }

        public class Response : PagedResult<Coin>
        {

        }

        [AutoMap(typeof(CoinEntity))]
        public class Coin
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string RefId { get; set; } = string.Empty;

            public string Symbol { get; set; } = string.Empty;

            public List<User> Users { get; set; } = new();
        }

        [AutoMap(typeof(UserEntity))]
        public class User
        {
            public Guid Id { get; set; }

            public string? Name { get; set; }

            public string? ProfileImageUrl { get; set; }
        }
    }
}