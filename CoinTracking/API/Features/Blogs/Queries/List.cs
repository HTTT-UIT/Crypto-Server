using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Blogs.Queries
{
    public class List
    {
        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly MasterContext _context;

            public Handler(MasterContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Blogs.AsNoTracking();

                var total = await query.CountAsync(cancellationToken);

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

        public class Query : PageQuery, IRequest<Response>
        {
        }

        public class Response : PagedResult<BlogEntity>
        {
        }
    }
}