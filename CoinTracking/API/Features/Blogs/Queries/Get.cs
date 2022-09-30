using API.Infrastructure;
using API.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Blogs.Queries
{
    public class Get
    {
        public class Handler : IRequestHandler<Query, BlogEntity>
        {
            private readonly MasterContext _context;

            public Handler(MasterContext context)
            {
                _context = context;
            }

            public async Task<BlogEntity?> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _context.Blogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                return item;
            }
        }

        public class Query : IRequest<BlogEntity>
        {
            [FromRoute]
            public int Id { get; set; }
        }
    }
}