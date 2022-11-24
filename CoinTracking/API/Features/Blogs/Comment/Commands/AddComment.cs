using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Comment.Commands
{
    public class AddComment
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

                var user = await _dbContext.Users
                    .Where(x => x.Id == command.ProfileId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (blog == null || user == null)
                {
                    return OperationResult.NotFound();
                }

                var comment = new CommentEntity()
                {
                    Content = command.Request.Content,
                    CommentTime = DateTime.Now,
                    User = user,
                    Blog = blog
                };

                await _dbContext.AddAsync(comment, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            [FromRoute]
            public int BlogId { get; set; }

            [FromBody]
            public Request Request { get; set; } = default!;
        }

        public class Request
        {
            [Required]
            public string Content { get; set; } = string.Empty;
        }
    }
}