namespace API.Infrastructure.Entities
{
    public class ViewedEntity
    {
        public ViewedEntity()
        {
            UserViewed = new UserEntity();
            CoinViewed = new CoinEntity();
        }

        public DateTime ViewTime { get; set; }
        
        public Guid UserId { get; set; }
        public virtual UserEntity UserViewed { get; set; }

        public Guid CoinId { get; set; }
        public virtual CoinEntity CoinViewed { get; set; }



    }
}
