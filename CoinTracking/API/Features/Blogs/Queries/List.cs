using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Blogs.Queries
{
    public class List
    {
        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly MasterContext _context;
            private readonly IMapper _mapper;

            public Handler(MasterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Blogs
                    .Include(i => i.Author)
                    .Include(i => i.FollowUsers)
                    .Include(i => i.Tags)
                    .AsNoTracking();

                var total = await query.CountAsync(cancellationToken);

                var items = await query
                    .Paginate(request)
                    .ToListAsync(cancellationToken);

                var result = _mapper.Map<List<ResponseItem>>(items);

                return new Response
                {
                    Items = result,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalRow = total
                };
            }
        }

        public class Query : PageQuery, IRequest<Response>
        {
        }

        public class Response : PagedResult<ResponseItem>
        {
        }

        public class ResponseItem : BaseEntity
        {
            public int Id { get; set; }

            public string Header { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public string AuthorName { get; set; } = string.Empty;

            public int TotalFollower { get; set; }

            public List<Tag> Tags { get; set; } = new();
        }

        public class Tag
        {
            public int Id { get; set; }

            public string Title { get; set; } = string.Empty;
        }
    }
}