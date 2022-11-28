using API.Infrastructure;
using API.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Shared.Services
{
    public class UserService : IUserService
    {
        private readonly MasterContext _context;

        public UserService(MasterContext context)
        {
            _context = context;
        }

        public async Task<List<UserEntity>> GetUser()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
    }
}