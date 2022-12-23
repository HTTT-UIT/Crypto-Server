using API.Common.Queries;
using API.Common.Result;
using API.Infrastructure.Entities.Common;
using API.Infrastructure;
using AutoMapper;
using MediatR;
using API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using API.Common.Extensions;

namespace API.Features.Reports.Queries
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
                var query = _context.Reports
                    .Include(i => i.UserReport)
                    .Include(i => i.BlogReport)
                    .AsNoTracking();

                if (request.Statuses != null && request.Statuses.Any())
                {
                    query = query.Where(i => request.Statuses.Contains(i.Status));
                }

                if (request.BlogId != null)
                {
                    query = query.Where(i => i.BlogReport.Id == request.BlogId);
                }

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
            public List<string>? Statuses { get; set; }

            public int? BlogId { get; set; }
        }

        public class Response : PagedResult<ResponseItem>
        {
        }

        [AutoMap(typeof(ReportEntity))]
        public class ResponseItem : BaseEntity
        {
            public int Id { get; set; }

            public string Reason { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public string Status { get; set; } = string.Empty;

            public Blog BlogReport { get; set; } = new();

            public User UserReport { get; set; } = new();
        }

        [AutoMap(typeof(BlogEntity))]
        public class Blog
        {
            public int Id { get; set; }

            public string Header { get; set; } = string.Empty;
        }

        [AutoMap(typeof(UserEntity))]
        public class User
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;
        }
    }
}
