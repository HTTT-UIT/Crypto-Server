using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoinBot.Helpers;
using CoinBot.Models;
using CoinBot.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        private readonly BotServices _botservices;
        #endregion

        public MainDialog(StateService stateService, BotServices botServices) : base(nameof(MainDialog))
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _botservices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));

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
            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _stateService));
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _stateService));
            AddDialog(new HotCoinDialog($"{nameof(MainDialog)}.hotCoin", _stateService));
            AddDialog(new FavoriteCoinDialog($"{nameof(MainDialog)}.favoriteCoin", _stateService));
            AddDialog(new BugTypeDialog($"{nameof(MainDialog)}.bugType", _botservices));
            AddDialog(new CoinDialog($"{nameof(MainDialog)}.coin", _botservices));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));
            

            // Set the starting Dialog
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // 4:08 6.12
            try
            {
                // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
                var recognizerResult = await _botservices.Dispatch.RecognizeAsync<LuisModel>(stepContext.Context, cancellationToken);


                // Top intent tell us which cognitive service to use.
                var topIntent = recognizerResult.TopIntent();
                
                switch(topIntent.intent)
                {
                    case LuisModel.Intent.GreetingIntent:
                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
                    case LuisModel.Intent.QueryCoinIntent:
                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.coin", null, cancellationToken);
                    case LuisModel.Intent.QueryHotCoinIntent:
                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.hotCoin", null, cancellationToken);
                    case LuisModel.Intent.QueryFavoriteCoinIntent:
                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.favoriteCoin", null, cancellationToken);
                    case LuisModel.Intent.NewBugReportIntent:
                        var userProfile = new UserProfile();
                        var bugReport = recognizerResult.Entities.BugReport_ML?.FirstOrDefault();
                        if (bugReport != null)
                        {
                            var description = bugReport.Description?.FirstOrDefault();
                            if (description != null)
                            {
                                userProfile.Description = bugReport._instance.Description?.FirstOrDefault() != null ? bugReport._instance.Description?.FirstOrDefault().Text : description.ToString();

                                // Retrieve Bug Text
                                var bugOuter = description.Bug?.FirstOrDefault();
                                if (bugOuter != null)
                                    userProfile.Bug = bugOuter[0] != null ? bugOuter[0] : userProfile.Bug;
                            }

                            // Retrieve Phone Number Text
                            userProfile.PhoneNumber = bugReport.PhoneNumber?.FirstOrDefault() != null ? bugReport.PhoneNumber?.FirstOrDefault() : userProfile.PhoneNumber;

                            // Retrieve Callback Time
                            userProfile.CallbackTime = bugReport.CallbackTime?.FirstOrDefault() != null ? AiRecognizer.RecognizeDateTime(bugReport.CallbackTime?.FirstOrDefault(), out var s) : userProfile.CallbackTime;
                        }

                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", userProfile, cancellationToken);
                    case LuisModel.Intent.QueryBugTypeIntent:
                        return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugType", null, cancellationToken);
                    default:
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Sorry, I don't understand."), cancellationToken);
                        return await stepContext.EndDialogAsync(null, cancellationToken);
                }    
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
