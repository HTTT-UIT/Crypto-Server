using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoinBot.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class BugTypeDialog : ComponentDialog
    {
        #region Variables
        private readonly BotServices _botServices;
        #endregion

        public BugTypeDialog(string dialogId, BotServices botServices) : base(dialogId)
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
            AddDialog(new WaterfallDialog($"{nameof(BugTypeDialog)}.mainFlow", waterfallSteps));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(BugTypeDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);
            var value = string.Empty;
            var bugOuter = result.Entities["BugTypes_List"][0];
            if(bugOuter!=null)
            {
                value = bugOuter[0] != null ? bugOuter[0].ToString() : string.Empty;
            }

            if (Common.BugTypes.Any(s => s.Equals(value, StringComparison.OrdinalIgnoreCase)))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"You have selected {value}"), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sorry, I didn't get that. Please select a valid bug type."), cancellationToken);
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
