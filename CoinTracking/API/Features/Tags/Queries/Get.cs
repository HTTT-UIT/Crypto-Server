using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Tags.Queries
{
    public class Get
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
                var item = await _context.Tags
                    .AsNoTracking()
                    .Include(i => i.Blogs)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                var result = _mapper.Map<Response>(item);

                return result;
            }
        }

        public class Query : IRequest<Response>
        {
            [FromRoute]
            public int Id { get; set; }
        }

        [AutoMap(typeof(TagEntity))]
        public class Response
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