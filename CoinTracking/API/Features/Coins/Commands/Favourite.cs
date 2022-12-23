using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Services;
using API.Infrastructure;
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
            private readonly ICoinService _coinService;

            public Handler(MasterContext dbContext, ICoinService coinService)
            {
                _dbContext = dbContext;
                _coinService = coinService;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var coin = await _dbContext.Coins
                    .Include(x => x.Users)
                    .Where(x => x.RefId == command.CoinRefId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                var user = await _dbContext.Users.Where(x => x.Id == command.Request.UserId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (user == null)
                {
                    return OperationResult.NotFound();
                }

                if (coin == null)
                {
                    var newCoinId = _coinService.SyncCoin(command.CoinRefId);

                    var newCoin = await _dbContext.Coins
                        .Include(x => x.Users)
                        .FirstOrDefaultAsync(x => x.Id == newCoinId, cancellationToken);

                    if (newCoin == null)
                    {
                        return OperationResult.BadRequest();
                    }

                    newCoin.Users.Add(user);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    return OperationResult.Ok();

                }

                if (coin.Users.Where(x => x.Id == command.Request.UserId).Any())
                {
                    coin.Users.Remove(user);
                }
                else
                {
                    coin.Users.Add(user);
                }
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            [FromRoute]
            public string CoinRefId { get; set; } = string.Empty;

            [FromBody]
            [Required]
            public Request Request { get; set; } = default!;
        }

        public class Request
        {
            [Required]
            public Guid UserId { get; set; }
        }
    }
}