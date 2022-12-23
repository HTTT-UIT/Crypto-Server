using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Users.Queries
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
                var query = _dbContext.Users.AsNoTracking()
                    .FilterDeleted();

                var total = await query.CountAsync(cancellationToken);

                if (!string.IsNullOrEmpty(request.Filter))
                {
                    query = query.Where(x => (x.Name ?? string.Empty).ToLower().Contains(request.Filter.ToLower()));
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
                    Items = _mapper.Map<List<ResponseItem>>(items),
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

        public class Response : PagedResult<ResponseItem>
        {
        }

        [AutoMap(typeof(UserEntity))]
        public class ResponseItem
        {
            public Guid Id { get; set; }

            public string UserName { get; set; } = string.Empty;

            public string? Name { get; set; }

            public DateTime? Dob { get; set; }

            public string? ProfileImageUrl { get; set; }
        }
    }
}