using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class SpecifiyDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;

        #endregion

        public SpecifiyDialog(string dialogId, StateService stateService) : base(dialogId)
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
            AddDialog(new WaterfallDialog($"{nameof(SpecifiyDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(SpecifiyDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(SpecifiyDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text($"Bạn nhập giúp mình tên coin nha");
            
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
