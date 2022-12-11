using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Services;
using API.Infrastructure;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Users.Commands
{
    public class UpdateAvatar
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult>
        {
            private readonly MasterContext _dbContext;
            private readonly IFileService _fileService;

            public Handler(MasterContext dbContext, IFileService fileService)
            {
                _dbContext = dbContext;
                _fileService = fileService;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await _dbContext.Users.FindAsync(new object?[] { command.Id }, cancellationToken: cancellationToken);

                if (user == null)
                {
                    return OperationResult.BadRequest();
                }

                using var mem = new MemoryStream();
                command.Image.CopyTo(mem);
                var extension = Path.GetExtension(command.Image.FileName);
                var imageName = Path.Combine(Guid.NewGuid().ToString(), extension);
                mem.Position = 0;
                user.ProfileImageUrl = await _fileService.UploadAsync(imageName, mem, cancellationToken);

                BaseUpdate(user, command);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            public Guid Id { get; set; }

            [Required]
            public IFormFile Image { get; set; } = default!;
        }
    }
}