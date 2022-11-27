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
    public class Update
    {
        public class Handler : BaseHandle, IRequestHandler<Command, OperationResult>
        {
            private readonly MasterContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(MasterContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<OperationResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = command.Request;

                var blog = await _dbContext.Blogs.FindAsync(new object?[] { command.Request.Id }, cancellationToken: cancellationToken);

                if (blog == null)
                {
                    return OperationResult.BadRequest();
                }

                _mapper.Map(request, blog);

                BaseUpdate(blog, command);
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
            public int Id { get; set; }

            public string? Header { get; set; }

            public string? Content { get; set; }
        }
    }
}