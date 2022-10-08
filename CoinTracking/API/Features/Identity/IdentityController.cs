using API.Features.Shared;
using API.Features.Shared.Models;
using API.Features.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Identity
{
    [Authorize]
    [Route("api/[controller]")]
    public class IdentityController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;

        public IdentityController(IMediator mediator, ITokenService tokenService) : base(mediator)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        public string Public()
        {
            return "The api to test identity";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            var token = await _tokenService.Authenticate(user);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}