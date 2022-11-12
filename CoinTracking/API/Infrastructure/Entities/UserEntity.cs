using System.ComponentModel.DataAnnotations;

namespace API.Infrastructure.Entities
{
    public class UserEntity
    {
        public UserEntity()
        {
            this.Coins = new HashSet<CoinEntity>();
            this.ViewedCoin = new HashSet<ViewedEntity>();
            this.FollowBlogs = new HashSet<BlogEntity>();
        }

        public Guid Id { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? Name { get; set; }

        public DateTime? Dob { get; set; }

        public virtual ICollection<CoinEntity> Coins { get; set; }

        public virtual ICollection<ViewedEntity> ViewedCoin { get; set; }

        public virtual ICollection<BlogEntity> FollowBlogs { get; set; }
    }
}