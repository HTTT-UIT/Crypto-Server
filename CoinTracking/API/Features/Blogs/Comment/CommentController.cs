using API.Features.Blogs.Comment.Commands;
using API.Features.Blogs.Comment.Queries;
using API.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Blogs.Comment
{
    [Route("api/Blog/{blogId}/[controller]")]
    public class CommentController : ApiControllerBase
    {
        public CommentController(IMediator mediator) : base(mediator)
        { }

        [HttpPost]
        public Task<IActionResult> Comment(AddComment.Command command)
            => HandleRequest(command);

        [HttpGet]
        public Task<IActionResult> ListComment(ListComments.Query query)
            => HandleRequest(query);

        [HttpDelete("{id}")]
        public Task<IActionResult> DeleteComment(DeleteComment.Command command)
            => HandleRequest(command);
    }
}