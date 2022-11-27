using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Blogs.Commands
{
    public class Create
    {
        public class Handler : IRequestHandler<Command, OperationResult<Response>>
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
                var blog = new BlogEntity
                {
                    Header = command.Request.Title,
                    Content = command.Request.Content,
                    AuthorId = command.Request.AuthorId,
                };

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