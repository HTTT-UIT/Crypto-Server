namespace API.Infrastructure.Entities
{
    public class CoinEntity
    {
        public CoinEntity()
        {
            this.Users = new HashSet<UserEntity>();
            this.ViewedUsers = new HashSet<ViewedEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public virtual ICollection<UserEntity> Users { get; set; }

        public virtual ICollection<ViewedEntity> ViewedUsers { get; set; }

    }
}