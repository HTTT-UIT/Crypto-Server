namespace API.Infrastructure.Entities
{
    public class BlogEntity
    {
        public BlogEntity()
        {
            FollowUsers = new List<UserEntity>();
        }

        public int Id { get; set; }

        public string Header { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Guid? AuthorId { get; set; }

        public virtual ICollection<UserEntity> FollowUsers { get; set; }
    }
}