using API.Infrastructure;
using API.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Coins.Queries
{
    public class Get
    {
        public class Handler : IRequestHandler<Query, CoinEntity?>
        {
            private readonly MasterContext _context;

            public Handler(MasterContext context)
            {
                _context = context;
            }

            public async Task<CoinEntity?> Handle(Query request, CancellationToken cancellationToken)
            {
                var item = await _context.Coins
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                return item;
            }
        }

        public class Query : IRequest<CoinEntity?>
        {
            [FromRoute]
            public Guid Id { get; set; }
        }
    }
}