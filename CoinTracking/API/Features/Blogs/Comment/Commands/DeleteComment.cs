using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure.Entities;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Blogs.Comment.Commands
{
    public class DeleteComment
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
                var blog = await _dbContext.Blogs
                    .Where(x => x.Id == command.BlogId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                var comment = await _dbContext.Comments
                    .Where(x => x.Id == command.Id)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (blog == null || comment == null)
                {
                    return OperationResult.NotFound();
                }

                _dbContext.Remove(comment);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            [FromRoute]
            public int BlogId { get; set; }

            [FromRoute]
            public int Id { get; set; }
        }
    }
}
