using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Coins.Queries
{
    public class Get
    {
        public class Handler : IRequestHandler<Query, Coin?>
        {
            private readonly MasterContext _context;
            private readonly IMapper _mapper;

            public Handler(MasterContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Coin?> Handle(Query request, CancellationToken cancellationToken)
            {
                bool isGuid = Guid.TryParse(request.Id, out Guid coinId);

                var query = _context.Coins.Include(i => i.Users)
                    .AsNoTracking();

                if (isGuid)
                {
                    query = query.Where(x => x.Id == coinId);
                }
                else
                {
                    query = query.Where(x => x.RefId == request.Id);
                }

                var item = await query.FirstOrDefaultAsync(cancellationToken);
                return _mapper.Map<Coin>(item);

            }
        }

        public class Query : IRequest<Coin?>
        {
            [FromRoute]
            public string Id { get; set; } = string.Empty;
        }


        [AutoMap(typeof(CoinEntity))]
        public class Coin
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string RefId { get; set; } = string.Empty;

            public string Symbol { get; set; } = string.Empty;

            public List<User> Users { get; set; } = new();
        }

        [AutoMap(typeof(UserEntity))]
        public class User
        {
            public Guid Id { get; set; }

            public string? Name { get; set; }

            public string? ProfileImageUrl { get; set; }
        }
    }
}