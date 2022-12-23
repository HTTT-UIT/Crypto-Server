using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using API.Infrastructure.Entities.Common;
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
                    .FilterDeleted()
                    .AsNoTracking();

                if (request.TagIds != null && request.TagIds.Any())
                {
                    query = query.Where(i => i.Tags.Any(o => request.TagIds.Any(t => o.Id == t)));
                }

                if (request.AuthorId.HasValue && request.AuthorId.Value != Guid.Empty)
                {
                    query = query.Where(i => i.AuthorId.HasValue
                        && i.AuthorId.Value == request.AuthorId.Value);
                }

                if (request.FollowerId.HasValue && request.FollowerId.Value != Guid.Empty)
                {
                    query = query.Where(i => i.FollowUsers.Any(f => f.Id == request.FollowerId));
                }

                if (request.Header != null)
                {
                    query = query.Where(i => i.Header.ToLower().Contains(request.Header.ToLower()));
                }

                var total = await query.CountAsync(cancellationToken);

                if (request.SortByFollow.HasValue && request.SortByFollow.Value)
                {
                    query = request.SortDir == SortDirection.Ascending
                        ? query.OrderBy(i => i.FollowUsers.Count)
                        : query.OrderByDescending(i => i.FollowUsers.Count);
                }
                else
                {
                    query = request.SortDir == SortDirection.Ascending
                        ? query.OrderBy(i => i.Id)
                        : query.OrderByDescending(i => i.Id);
                }

                if (request.Status == null || !request.Status.Any())
                {
                    query = query.Where(i => i.Status == BlogStatus.ACCEPTED || i.Status == BlogStatus.WARNED);
                }
                else
                {
                    query = query.Where(i => request.Status.Contains((int)i.Status));
                }

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
            public List<int>? TagIds { get; set; }

            public bool? SortByFollow { get; set; }

            public string? SortDir { get; set; }

            public Guid? AuthorId { get; set; }

            public Guid? FollowerId { get; set; }

            public string? Header { get; set; }

            public List<int>? Status { get; set; }
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

            public Guid AuthorId { get; set; }

            public string? AuthorImageUrl { get; set; }

            public int TotalFollower { get; set; }

            public List<Tag> Tags { get; set; } = new();

            public bool Deleted { get; set; }

            public string? SubContent { get; set; }

            public string? ImageUrl { get; set; }

            public int Status { get; set; }
        }

        public class Tag
        {
            public int Id { get; set; }

            public string Title { get; set; } = string.Empty;
        }
    }
}