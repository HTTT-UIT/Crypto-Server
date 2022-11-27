using API.Features.Shared;
using API.Features.Tags.Commands;
using API.Features.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Tags
{
    [Route("api/[controller]")]
    public class TagController : ApiControllerBase
    {
        public TagController(IMediator mediator) : base(mediator)
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

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Delete.Command command)
            => HandleRequest(command);
    }
}