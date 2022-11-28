using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

            public Handler(MasterContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<OperationResult<Response>> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = command.Request;

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
                };

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
            [FromBody]
            [Required]
            public Request Request { get; set; } = default!;
        }

        public class Request
        {
            [Required]
            [MinLength(1)]
            public string Title { get; set; } = string.Empty;

            [Required]
            public string Content { get; set; } = string.Empty;

            public Guid? AuthorId { get; set; }

            public List<int>? TagIds { get; set; }
        }

        [AutoMap(typeof(BlogEntity))]
        public class Response
        {
            public int Id { get; set; }

            public string Header { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;

            public Guid? AuthorId { get; set; }
        }
    }
}