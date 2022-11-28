using API.Common.Commands;
using API.Common.Result;
using API.Infrastructure;
using API.Infrastructure.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Features.Coins.Commands
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
                var coin = new CoinEntity
                {
                    Name = command.Request.Name,
                };

                BaseCreate(coin, command);
                _dbContext.Add(coin);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var response = _mapper.Map<Response>(coin);

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
            [MinLength(1)]
            public string Name { get; set; } = string.Empty;
        }

        [AutoMap(typeof(CoinEntity))]
        public class Response
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}