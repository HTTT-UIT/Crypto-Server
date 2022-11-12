using API.Common.Commands;
using API.Common.Result;
using API.Common.Utils;
using API.Features.Shared.Constants;
using API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Users.Commands
{
    public class Update
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
                var request = command.Request;

                var user = await _dbContext.Users.FindAsync(new object?[] { command.Request.UserId }, cancellationToken: cancellationToken);

                if (user == null)
                {
                    return OperationResult.BadRequest();
                }

                user.Name = request.Name ?? user.Name;
                user.Dob = request.Dob ?? user.Dob;

                if (request.Role == UserRole.Admin || request.Role == UserRole.Blogger || request.Role == UserRole.User)
                {
                    user.Role = request.Role;
                }
                else
                {
                    user.Role = UserRole.Guest;
                }

                if (request.Password != null)
                {
                    user.Password = Password.Hash(request.Password);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

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

            public string? Password { get; set; }

            public string? Role { get; set; }

            public string? Name { get; set; }
            
            public DateTime? Dob { get; set; }
        }
    }
}