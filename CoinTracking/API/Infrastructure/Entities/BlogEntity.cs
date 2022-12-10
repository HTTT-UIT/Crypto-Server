using API.Infrastructure.Entities.Common;

namespace API.Infrastructure.Entities
{
    public class BlogEntity : BaseEntity, ISoftEntity
    {
        public BlogEntity()
        {
            this.FollowUsers = new List<UserEntity>();
            this.Comments = new List<CommentEntity>();
            this.Tags = new List<TagEntity>();
            this.Reports = new HashSet<ReportEntity>();
        }

        public int Id { get; set; }

        public string Header { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Guid? AuthorId { get; set; }

        public string SubContent { get; set; } = string.Empty;

        public string? ImageUrl { get; set; } = string.Empty;

        public virtual UserEntity? Author { get; set; }

        public virtual ICollection<UserEntity> FollowUsers { get; set; }

        public virtual ICollection<CommentEntity> Comments { get; set; }

        public virtual ICollection<TagEntity> Tags { get; set; }

        public virtual ICollection<ReportEntity> Reports { get; set; }

        public bool Deleted { get; set; }
    }
}