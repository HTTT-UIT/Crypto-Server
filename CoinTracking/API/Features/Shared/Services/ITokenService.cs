using API.Features.Shared.Models;
using API.Infrastructure.Entities;

namespace API.Features.Shared.Services
{
    public interface ITokenService
    {
        Task<Tokens?> Authenticate(User user);
    }
}