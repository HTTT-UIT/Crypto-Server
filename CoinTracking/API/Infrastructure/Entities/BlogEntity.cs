using API.Infrastructure.Entities.Common;

namespace API.Infrastructure.Entities
{
    public class BlogEntity : BaseEntity
    {
        public BlogEntity()
        {
            FollowUsers = new List<UserEntity>();
            Comments = new List<CommentEntity>();
            Tags = new List<TagEntity>();
        }

        public int Id { get; set; }

        public string Header { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Guid? AuthorId { get; set; }

        public virtual UserEntity? Author { get; set; }

        public virtual ICollection<UserEntity> FollowUsers { get; set; }

        public virtual ICollection<CommentEntity> Comments { get; set; }

        public virtual ICollection<TagEntity> Tags { get; set; }
    }
}