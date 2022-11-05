using API.Infrastructure.Entities;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Users.Queries
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
                var item = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                return _mapper.Map<Response>(item);
            }
        }

        public class Query : IRequest<Response?>
        {
            [FromRoute]
            public Guid Id { get; set; }
        }

        [AutoMap(typeof(UserEntity))]
        public class Response
        {
            public Guid Id { get; set; }
            public string UserName { get; set; } = string.Empty;
            public string? Name { get; set; }
            public DateTime? Dob { get; set; }
        }
    }
}
