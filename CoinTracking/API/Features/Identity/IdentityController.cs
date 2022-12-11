using API.Common.Utils;
using API.Features.Shared;
using API.Features.Shared.Constants;
using API.Features.Shared.Models;
using API.Features.Shared.Services;
using API.Infrastructure;
using API.Infrastructure.Entities;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Identity
{
    [Authorize(Roles = UserRole.User)]
    [Route("api/[controller]")]
    public class IdentityController : ApiControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly MasterContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public IdentityController(IMediator mediator, ITokenService tokenService, MasterContext context, BlobServiceClient blobServiceClient) : base(mediator)
        {
            _tokenService = tokenService;
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        [HttpGet]
        public string Public()
        {
            return string.Empty;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> SignUp([FromBody] User user)
        {
            var exists = await _context.Users.AnyAsync(x => x.UserName == user.UserName);

            if (exists)
            {
                return Conflict();
            }

            var newUser = new UserEntity()
            {
                UserName = user.UserName,
                Password = Password.Hash(user.Password),
                Role = UserRole.User,
            };

            newUser.CreatedBy = Resource.SYSTEM;
            newUser.CreatedAt = DateTime.Now;

            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}