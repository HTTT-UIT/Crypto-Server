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

        public async Task<UserEntity> GetByCredential(string userName, string password)
        {
            return new UserEntity();
        }

        public async Task<List<UserEntity>> GetList()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
    }
}