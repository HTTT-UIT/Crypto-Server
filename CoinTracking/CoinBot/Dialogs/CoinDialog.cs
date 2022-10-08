using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class CoinDialog : ComponentDialog
    {
        #region Variables
        private readonly BotServices _botServices;
        #endregion
        
        public CoinDialog(string dialogId, BotServices botServices) : base(dialogId)
        {
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            // Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(CoinDialog)}.mainFlow", waterfallSteps));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(CoinDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            var coinOuter = result.Entities["Coin_ML"][0];
            /*if (coinOuter != null)
            {
                value = coinOuter[0] != null ? coinOuter[0].ToString() : string.Empty;
            }*/

            if (coinOuter != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You want to tracking about {coinOuter["Coin"][0]}"), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, I didn't get that"), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
