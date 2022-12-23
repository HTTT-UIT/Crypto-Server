using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Models;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Reports.Commands
{
    public class UpdateStatus
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
                var report = await _dbContext.Reports
                    .Where(x => x.Id == command.Id)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (report == null)
                {
                    return OperationResult.NotFound();
                }

                report.Status = command.Request.Status;
                BaseUpdate(report, command);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            [FromRoute]
            public int Id { get; set; }

            [FromBody]
            public Request Request { get; set; } = new();
        }

        public class Request
        {
            public string Status { get; set; } = string.Empty;
        }
    }
}
