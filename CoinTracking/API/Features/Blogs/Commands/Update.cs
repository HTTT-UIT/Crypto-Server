using API.Common.Commands;
using API.Common.Result;
using API.Features.Shared.Services;
using API.Infrastructure;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Commands
{
    public class Update
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult>
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

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = command;

                var blog = await _dbContext.Blogs
                    .Include(i => i.Tags)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken: cancellationToken);

                if (blog == null)
                {
                    return OperationResult.BadRequest();
                }

                _mapper.Map(request, blog);

                if (request.TagIds != null && request.TagIds.Any())
                {
                    foreach (var tagId in request.TagIds)
                    {
                        var tag = await _dbContext.Tags.FindAsync(tagId);

                        if (tag != null && !blog.Tags.Any(t => t.Id == tagId))
                        {
                            blog.Tags.Add(tag);
                        }
                    }

                    var currentTags = blog.Tags.ToList();

                    foreach (var tag in currentTags)
                    {
                        if (!request.TagIds.Contains(tag.Id))
                        {
                            blog.Tags.Remove(tag);
                        }
                    }
                }

                if (request.Image != null)
                {
                    using var mem = new MemoryStream();
                    request.Image.CopyTo(mem);
                    var extension = Path.GetExtension(request.Image.FileName);
                    var imageName = Path.Combine(Guid.NewGuid().ToString(), extension);
                    mem.Position = 0;
                    blog.ImageUrl = await _fileService.UploadAsync(imageName, mem, cancellationToken);
                }

                BaseUpdate(blog, command);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return OperationResult.Ok();
            }
        }

        public class Command : BaseCommand, IRequest<OperationResult>
        {
            [Required]
            public int Id { get; set; }

            public string? Header { get; set; }

            public string? Content { get; set; }

            public List<int>? TagIds { get; set; }

            public string? SubContent { get; set; }

            public IFormFile? Image { get; set; }

            public int Status { get; set; }
        }
    }
}