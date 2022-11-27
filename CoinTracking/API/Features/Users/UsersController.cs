using API.Features.Shared;
using API.Features.Users.Commands;
using API.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Users
{
    [Route("api/[controller]")]
    public class UsersController : ApiControllerBase
    {
        public UsersController(IMediator mediator) : base(mediator)
        { }

        [HttpGet]
        public Task<IActionResult> List(List.Query query)
            => HandleRequest(query);

        [HttpGet("{id}")]
        public Task<IActionResult> Get(Get.Query query)
            => HandleRequest(query);

        [HttpPut]
        public Task<IActionResult> Update(Update.Command command)
            => HandleRequest(command);
    }
}