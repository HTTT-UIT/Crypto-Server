using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class AbilitiesDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;

        #endregion

        public AbilitiesDialog(string dialogId, StateService stateService) : base(dialogId)
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
            AddDialog(new WaterfallDialog($"{nameof(AbilitiesDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(AbilitiesDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(AbilitiesDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text($"Các tính năng của bot là:");
            reply.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>
                {
                    new CardAction { Title = Common.Abilities[0], Type = ActionTypes.ImBack, Value = Common.Abilities[0] },
                    new CardAction { Title = Common.Abilities[1], Type = ActionTypes.ImBack, Value = Common.Abilities[1] },
                    new CardAction { Title = Common.Abilities[2], Type = ActionTypes.ImBack, Value = Common.Abilities[2] },
                },
            };
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
