using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace API.Features.Shared
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IMediator _mediator;

        public ApiControllerBase(IMediator mediator)
           => _mediator = mediator;

        protected async Task<IActionResult> HandleRequest<TResponse>(IRequest<TResponse> request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(request);

            return ConvertToActionResult(result ?? throw new InvalidOperationException());
        }

        protected IActionResult ConvertToActionResult(object result)
        {
            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}