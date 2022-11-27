using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Commands
{
    public class Follow
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
                    .Include(x => x.FollowUsers)
                    .Where(x => x.Id == command.Request.BlogId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                var user = await _dbContext.Users.Where(x => x.Id == command.Request.UserId)
                    .FirstOrDefaultAsync();

                if (blog == null || user == null)
                {
                    return OperationResult.NotFound();
                }

                if (!blog.FollowUsers.Where(x => x.Id == command.Request.UserId).Any())
                {
                    blog.FollowUsers.Add(user);
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

            public int BlogId { get; set; }
        }
    }
}