using API.Features.Shared.Models;

namespace API.Features.Shared.Services
{
    public interface ITokenService
    {
        Task<Tokens?> Authenticate(User user);
    }
}