using API.Common.Extensions;
using API.Common.Queries;
using API.Common.Result;
using API.Features.Shared.Constants;
using API.Infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Blogs.Comment.Queries
{
    public class ListComments
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
                var query = _dbContext.Comments.AsNoTracking();

                var total = await query.CountAsync(cancellationToken);

                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = request.SortDir == SortDirection.Descending
                        ? query.OrderByDescending(request.SortBy)
                        : query.OrderBy(request.SortBy);
                }

                var items = await query
                    .Include(x => x.User)
                    .Paginate(request)
                    .ToListAsync(cancellationToken);

                var res = _mapper.Map<List<CommentViewModel>>(items);

                return new Response
                {
                    Items = res,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalRow = total
                };
            }
        }

        public class Query : OrderQuery, IRequest<Response>
        {
        }

        public class Response : PagedResult<CommentViewModel>
        {
        }

        public class CommentViewModel
        {
            public int Id { get; set; }

            public string Content { get; set; } = string.Empty;

            public DateTime CommentTime { get; set; }

            public Guid UserId { get; set; }

            public string Username { get; set; } = string.Empty;
        }
    }
}