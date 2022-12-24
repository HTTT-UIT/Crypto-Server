using CoinBot.Helpers;
using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using RestSharp;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class CoinDialog : ComponentDialog
    {
        #region Variables
        private readonly BotServices _botServices;
        private CoinMarketCapApi _coinMarketCapApi;
        private Helpers.Common _common;
        #endregion

        public CoinDialog(string dialogId, BotServices botServices, CoinMarketCapApi coinMarketCapApi) : base(dialogId)
        {
            _botServices = botServices ?? throw new System.ArgumentNullException(nameof(botServices));
            _coinMarketCapApi = coinMarketCapApi ?? throw new System.ArgumentNullException(nameof(coinMarketCapApi));
            _common = new Helpers.Common();

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
                var coinData = _coinMarketCapApi.MakeAPICall(string.Format("/v2/cryptocurrency/info?slug={0}", coinOuter["Coin"][0].ToString().ToLower()));

                // Create a HeroCard with options for the user to interact with the bot.
                var card = new HeroCard
                {
                    Title = coinData["data"].First.First["name"].ToString(),
                    Text = Translator.Translate(coinData["data"].First.First["description"].ToString()).Result,
                    Images = new List<CardImage> { new CardImage(coinData["data"].First.First["logo"].ToString()) },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.OpenUrl, title: "Xem thêm", value: "https://coinmarketcap.com/currencies/" + coinData["data"].First.First["slug"].ToString()),
                    },
                };
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(card.ToAttachment()), cancellationToken);
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
