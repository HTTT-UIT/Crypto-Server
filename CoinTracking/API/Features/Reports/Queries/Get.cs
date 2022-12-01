using API.Infrastructure.Entities.Common;
using API.Infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Infrastructure.Entities;

namespace API.Features.Reports.Queries
{
    public class Get
    {
        public class Handler : IRequestHandler<Query, Response?>
        {
            private readonly MasterContext _context;
            private readonly IMapper _mapper;

            public Handler(MasterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Response?> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _context.Reports
                    .Include(i => i.BlogReport)
                    .Include(i => i.UserReport)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                var result = _mapper.Map<Response>(item);

                return result;
            }
        }

        public class Query : IRequest<Response?>
        {
            [FromRoute]
            public int Id { get; set; }
        }

        [AutoMap(typeof(ReportEntity))]
        public class Response : BaseEntity
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
