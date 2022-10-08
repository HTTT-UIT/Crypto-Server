using CoinBot.Models;
using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class FavoriteCoinDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;

        #endregion

        public FavoriteCoinDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(FavoriteCoinDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(FavoriteCoinDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(FavoriteCoinDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You want to tracking about you favorite coin"), cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}