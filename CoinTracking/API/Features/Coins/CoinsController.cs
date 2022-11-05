using API.Features.Coins.Commands;
using API.Features.Coins.Queries;
using API.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Coins
{
    [Route("api/[controller]")]
    public class CoinsController : ApiControllerBase
    {
        public CoinsController(IMediator mediator) : base(mediator)
        { }

        [HttpGet]
        public Task<IActionResult> List(List.Query query)
            => HandleRequest(query);

        [HttpGet("{id}")]
        public Task<IActionResult> Get(Get.Query query)
            => HandleRequest(query);

        [HttpPost]
        public Task<IActionResult> Create(Create.Command command)
            => HandleRequest(command);
    }
}