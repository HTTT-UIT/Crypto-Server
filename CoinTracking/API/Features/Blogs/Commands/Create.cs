using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Services;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Commands
{
    public class Create
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult<Response>>
        {
            private readonly MasterContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IFileService _fileService;

            public Handler(MasterContext dbContext, IMapper mapper, IFileService fileService)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _fileService = fileService;
            }

            public async Task<OperationResult<Response>> Handle(Command command, CancellationToken cancellationToken)
            {
                //to prevent confuse
                var request = command;

                var author = await _dbContext.Users
                    .Where(x => x.Id == request.AuthorId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (author == null)
                {
                    return OperationResult.BadRequest(Resource.AUTHOR_NOT_FOUND);
                }

                var blog = new BlogEntity
                {
                    Header = request.Title,
                    Content = request.Content,
                    AuthorId = request.AuthorId,
                    SubContent = request.SubContent
                };

                if (request.Image != null)
                {
                    using var mem = new MemoryStream();
                    request.Image.CopyTo(mem);
                    var extension = Path.GetExtension(request.Image.FileName);
                    var imageName = Path.Combine(Guid.NewGuid().ToString(), extension);
                    mem.Position = 0;
                    blog.ImageUrl = await _fileService.UploadAsync(imageName, mem, cancellationToken);
                }

                if (request.TagIds != null && request.TagIds.Any())
                {
                    foreach (var tagId in request.TagIds)
                    {
                        var tag = await _dbContext.Tags.FindAsync(tagId);
                        if (tag != null)
                        {
                            blog.Tags.Add(tag);
                        }
                    }
                }

                BaseCreate(blog, command);
                _dbContext.Add(blog);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var response = _mapper.Map<Response>(blog);

                return OperationResult.Created(response);
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult<Response>>
        {
            [Required]
            [MinLength(1)]
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Content { get; set; } = string.Empty;

            public string? SubContent { get; set; }

            public Guid? AuthorId { get; set; }

            public List<int>? TagIds { get; set; }

            public IFormFile? Image { get; set; }
        }

        [AutoMap(typeof(BlogEntity))]
        public class Response
        {
            public int Id { get; set; }

            public string Header { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public Guid? AuthorId { get; set; }

            public string? SubContent { get; set; }

            public string? ImageUrl { get; set; } = string.Empty;
        }
    }
}