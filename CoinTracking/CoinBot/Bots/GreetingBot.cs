using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using CoinBot.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Bots
{
    public class GreetingBot : ActivityHandler
    {
        private readonly StateService _stateService;

        public GreetingBot(StateService stateService)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
        }

        public async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var userProfile = await _stateService.UserProfileAccessor.GetAsync(turnContext, () => new Models.UserProfile());
            var conversationData = await _stateService.ConversationDataAccessor.GetAsync(turnContext, () => new Models.ConversationData());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                if (conversationData.PromptedUserForName)
                {
                    // Set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that we got their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Thanks {userProfile.Name}. How can I help you today?"), cancellationToken);

                    // Reset the flag to allow the bot to go though the cycle again
                    conversationData.PromptedUserForName = false;
                }
                else
                {
                    // Prompt the user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name???"), cancellationToken);

                    // Set the flag to true, so we don't prompt in the next turn
                    conversationData.PromptedUserForName = true;
                }

                // Save any state changes that might have occured during the turn
                await _stateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _stateService.UserState.SaveChangesAsync(turnContext);
                await _stateService.ConversationState.SaveChangesAsync(turnContext);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hi {userProfile.Name}. How can I help you today?"), cancellationToken);
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }
    }
}
