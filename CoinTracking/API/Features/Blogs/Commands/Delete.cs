using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Models;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Commands
{
    public class Delete
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult>
        {
            private readonly MasterContext _dbContext;

            public Handler(MasterContext dbContext, IApplicationUser applicationUser) : base(applicationUser)
            {
                _dbContext = dbContext;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var blog = await _dbContext.Blogs
                    .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken: cancellationToken);

                if (blog == null)
                {
                    return OperationResult.NotFound();
                }

                BaseDelete(blog, command);
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
