using API.Features.Blogs.Commands;
using API.Features.Blogs.Queries;
using API.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Blogs
{
    [Route("api/[controller]")]
    public class BlogsController : ApiControllerBase
    {
        public BlogsController(IMediator mediator) : base(mediator)
        { }

        [HttpGet]
        public Task<IActionResult> List(List.Query query)
            => HandleRequest(query);

        [HttpGet("{id}")]
        public Task<IActionResult> Get(Get.Query query)
            => HandleRequest(query);

        [HttpPost]
        public Task<IActionResult> Create([FromForm] Create.Command command)
            => HandleRequest(command);

        [HttpPost("Follow")]
        public Task<IActionResult> Follow(Follow.Command command)
             => HandleRequest(command);

        [HttpPut]
        public Task<IActionResult> Update(Update.Command command)
            => HandleRequest(command);

        [HttpDelete("{id}")]
        public Task<IActionResult> Delete(Delete.Command command)
            => HandleRequest(command);
    }
}