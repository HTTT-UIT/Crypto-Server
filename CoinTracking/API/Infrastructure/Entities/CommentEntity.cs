namespace API.Infrastructure.Entities
{
    public class CommentEntity
    {
        public CommentEntity ()
        {
            Blog = new BlogEntity();
            User = new UserEntity();
        }

        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime CommentTime { get; set; }

        public BlogEntity Blog { get; set; }

        public UserEntity User { get; set; }
    }
}
