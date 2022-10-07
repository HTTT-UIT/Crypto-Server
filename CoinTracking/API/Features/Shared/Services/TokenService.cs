using API.Features.Shared.Models;
using API.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Features.Shared.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _iconfiguration;
        private readonly IUserService _userService;

        public TokenService(IConfiguration iconfiguration, IUserService userService)
        {
            _iconfiguration = iconfiguration;
            _userService = userService;
        }

        public async Task<Tokens?> Authenticate(User user)
        {
            var users = await _userService.GetUser();
            if (!users.Any(x => x.UserName == user.UserName && x.Password == user.Password))
            {
                return null;
            }

            // Generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_iconfiguration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName)
                }),
                Issuer = _iconfiguration["Jwt:Issuer"],
                Audience = _iconfiguration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens { Token = tokenHandler.WriteToken(token) };
        }
    }
}