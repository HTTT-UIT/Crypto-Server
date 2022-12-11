using System.ComponentModel.DataAnnotations;
using API.Infrastructure.Entities.Common;

namespace API.Infrastructure.Entities
{
    public class UserEntity : BaseEntity
    {
        public UserEntity()
        {
            this.Coins = new HashSet<CoinEntity>();
            this.ViewedCoin = new HashSet<ViewedEntity>();
            this.FollowBlogs = new HashSet<BlogEntity>();
            this.Blogs = new HashSet<BlogEntity>();
            this.Reports = new HashSet<ReportEntity>();
        }

        public Guid Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? Name { get; set; }

        public DateTime? Dob { get; set; }

        public string? ProfileImageUrl { get; set; }

        public virtual ICollection<CoinEntity> Coins { get; set; }

        public virtual ICollection<ViewedEntity> ViewedCoin { get; set; }

        public virtual ICollection<BlogEntity> FollowBlogs { get; set; }

        public virtual ICollection<BlogEntity> Blogs { get; set; }

        public virtual ICollection<ReportEntity> Reports { get; set; }
    }
}