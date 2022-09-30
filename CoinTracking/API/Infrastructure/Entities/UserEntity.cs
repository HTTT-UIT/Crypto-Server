namespace API.Infrastructure.Entities
{
    public class UserEntity
    {
        public string UserName { get; set; } = string.Empty;

        public string HashPassword { get; set; } = string.Empty;
    }
}