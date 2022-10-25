using System.ComponentModel.DataAnnotations;

namespace API.Infrastructure.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}