using Microsoft.Bot.Builder;
using System;

namespace CoinBot.Models
{
    public class ConversationData
    {
        public bool PromptedUserForName { get; set; } = false;

        public static implicit operator ConversationData(ConversationState v)
        {
            throw new NotImplementedException();
        }
    }
}
