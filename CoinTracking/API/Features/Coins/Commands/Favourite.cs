using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Coins.Commands
{
    public class Favourite
    {
        public class Handler : IRequestHandler<Command, OperationResult>
        {
            private readonly MasterContext _dbContext;

            public Handler(MasterContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var coin = await _dbContext.Coins
                    .Include(x => x.Users)
                    .Where(x => x.Id == command.Request.CoinId)
                    .FirstOrDefaultAsync();
                var user = await _dbContext.Users.Where(x => x.Id == command.Request.UserId)
                    .FirstOrDefaultAsync();

                if (coin == null || user == null)
                {
                    return OperationResult.NotFound();
                }

                if (!coin.Users.Where(x => x.Id == command.Request.UserId).Any())
                {
                    coin.Users.Add(user);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [FromBody]
            [Required]
            public Request Request { get; set; } = default!;
        }

        public class Request
        {
            [Required]
            public Guid UserId { get; set; }
            public Guid CoinId { get; set; }
        }
    }
}
