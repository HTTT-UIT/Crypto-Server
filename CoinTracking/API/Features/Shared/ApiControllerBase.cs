using API.Common.Enums;
using API.Common.Result;
using API.Features.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using BadRequestResult = API.Common.Result.BadRequestResult;

namespace API.Features.Shared
{
    [Consumes(MediaTypeNames.Application.Json, "multipart/form-data")]
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

            if (result == null)
            {
                return NotFound();
            }

            return ConvertToActionResult(result ?? throw new InvalidOperationException());
        }

        protected async Task<IActionResult> HandleRequest(IRequest<OperationResult> request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(request);

            return ConvertToActionResult(result);
        }

        protected IActionResult ConvertToActionResult<TResponse>(OperationResult<TResponse> result)
        {
            if (!result.Succeeded)
            {
                if (result.Status.StatusCode == OperationResultStatusCode.BadRequest)
                {
                    var badRequestResult = result as BadRequestResult<TResponse>;

                    foreach (var validationResult in badRequestResult?.ValidationResults ?? Enumerable.Empty<ValidationResult?>())
                        foreach (var memberName in validationResult?.MemberNames ?? Enumerable.Empty<string>())
                            ModelState.AddModelError(memberName, validationResult?.ErrorMessage ?? string.Empty);

                    return BadRequest(ModelState);
                }

                return GetErrorActionResult(result.Status);
            }

            if (result.Status.StatusCode == OperationResultStatusCode.Created)
            {
                var createdResult = result as CreatedResult<TResponse>;

                if (string.IsNullOrWhiteSpace(createdResult?.Id) == false)
                {
                    var resource = GetResourcePath(createdResult.Id);

                    return Created(resource, createdResult.Value);
                }
            }

            if (result.Value != null)
                return new ObjectResult(result.Value)
                {
                    StatusCode = (int)result.Status.StatusCode
                };
            else
                return new StatusCodeResult((int)result.Status.StatusCode);
        }

        protected IActionResult ConvertToActionResult(OperationResult result)
        {
            if (!result.Succeeded)
            {
                if (result is BadRequestResult badRequestResult)
                {
                    foreach (var validationResult in badRequestResult.ValidationResults ?? Enumerable.Empty<ValidationResult>())
                        foreach (var memberName in validationResult?.MemberNames ?? Enumerable.Empty<string>())
                            ModelState.AddModelError(memberName, validationResult?.ErrorMessage ?? string.Empty);

                    return BadRequest(ModelState);
                }

                return GetErrorActionResult(result.Status);
            }

            return new StatusCodeResult((int)result.Status.StatusCode);
        }

        private IActionResult GetErrorActionResult(OperationStatus status)
        {
            if (status.HasDetails())
            {
                var result = new ObjectResult(status)
                {
                    StatusCode = (int)status.StatusCode
                };

                return result;
            }
            else
            {
                var result = new StatusCodeResult((int)status.StatusCode);

                return result;
            }
        }

        private PathString GetResourcePath(object resource)
            => Request.Path.Add(PathString.FromUriComponent($"/{resource}"));

        protected IActionResult ConvertToActionResult(object result)
        {
            if (result is IActionResult)
            {
                return result as IActionResult ?? BadRequest();
            }

            if (result == null)
                return NoContent();

            return Ok(result);
        }
    }
}