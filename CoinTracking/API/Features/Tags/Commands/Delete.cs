using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Tags.Commands
{
    public class Delete
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult>
        {
            private readonly MasterContext _dbContext;

            public Handler(MasterContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var tag = await _dbContext.Tags
                    .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

                if (tag == null)
                {
                    return OperationResult.NotFound();
                }

                BaseDelete(tag);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            [FromRoute]
            public int Id { get; set; }
        }
    }
}