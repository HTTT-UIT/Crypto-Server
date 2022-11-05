using System;

namespace CoinBot.Models
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime? CallbackTime { get; set; }
        public string PhoneNumber { get; set; }
        public string Bug { get; set; }
        public string Coin { get; set; }
    }
}
