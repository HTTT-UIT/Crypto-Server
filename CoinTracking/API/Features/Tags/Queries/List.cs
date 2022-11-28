using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Tags.Queries
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
                var query = _dbContext.Tags
                    .AsNoTracking()
                    .FilterDeleted();

                var total = await query.CountAsync(cancellationToken);

                if (!string.IsNullOrEmpty(request.Filter))
                {
                    query = query.Where(x => x.Title.ToLower().Contains(request.Filter.ToLower()));
                }

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.SortDir == SortDirection.Descending
                        ? query.OrderByDescending(request.SortBy)
                        : query.OrderBy(request.SortBy);
                }

                var tags = await query
                    .Include(x => x.Blogs)
                    .Paginate(request)
                    .ToListAsync(cancellationToken);

                var items = _mapper.Map<List<ResponseItem>>(tags);

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
        }

        public class Response : PagedResult<ResponseItem>
        {
        }

        [AutoMap(typeof(TagEntity))]
        public class ResponseItem
        {
            public int Id { get; set; }

            public string Title { get; set; } = string.Empty;

            public bool Deleted { get; set; }

            public List<Blog> Blogs { get; set; } = new();
        }

        [AutoMap(typeof(BlogEntity))]
        public class Blog
        {
            public int Id { get; set; }

            public string Header { get; set; } = string.Empty;
        }
    }
}