using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure.Entities;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using API.Features.Shared.Constants;
using API.Features.Shared.Models;

namespace API.Features.Reports.Commands
{
    public class Create
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
                    .Where(x => x.Id == command.Request.BlogId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                var user = await _dbContext.Users
                    .Where(x => x.Id == command.ProfileId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (blog == null || user == null)
                {
                    return OperationResult.NotFound();
                }

                var report = new ReportEntity()
                {
                    Content = command.Request.Content,
                    Reason = command.Request.Reason,
                    UserReport = user,
                    BlogReport = blog,
                    Status = ReportStatus.New
                };

                if (blog.Status == BlogStatus.ACCEPTED)
                {
                    blog.Status = BlogStatus.WARNED;
                }

                BaseCreate(report, command);
                await _dbContext.AddAsync(report, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [FromBody]
            public Request Request { get; set; } = default!;
        }

        public class Request
        {
            [Required]
            public int BlogId { get; set; }

            [Required]
            public string Reason { get; set; } = string.Empty;

            [Required]
            public string Content { get; set; } = string.Empty;
        }
    }
}
