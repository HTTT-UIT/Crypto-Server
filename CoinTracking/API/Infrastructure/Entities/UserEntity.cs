using API.Features.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace API.Infrastructure.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public UserRole Role { get; set; }
    }
}