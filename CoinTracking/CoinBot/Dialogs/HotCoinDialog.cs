using AdaptiveCards;
using CoinBot.Helpers;
using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class HotCoinDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        private CoinMarketCapApi _coinMarketCapApi;

        #endregion

        public HotCoinDialog(string dialogId, StateService stateService, CoinMarketCapApi coinMarketCapApi) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _coinMarketCapApi = coinMarketCapApi ?? throw new System.ArgumentNullException(nameof(coinMarketCapApi));

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
            AddDialog(new WaterfallDialog($"{nameof(HotCoinDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(HotCoinDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(HotCoinDialog)}.mainFlow";
        }

        [System.Obsolete]
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var coinData = _coinMarketCapApi.MakeAPICall(string.Format("/v1/cryptocurrency/listings/latest?start=1&limit=5&sort=market_cap&cryptocurrency_type=all&tag=all"));
            List<Attachment> attachments = new();

            foreach (var coin in coinData["data"].Children())
            {
                var card = SendCard(coin);
                attachments.Add(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card

                });
            }

            var bigCard = new Activity()
            {
                AttachmentLayout = AttachmentLayoutTypes.Carousel,
                Attachments = attachments,
                Type = ActivityTypes.Message
            };

            await stepContext.Context.SendActivityAsync(bigCard, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private AdaptiveCard SendCard(JToken coin)
        {
            var data = _coinMarketCapApi.MakeAPICall(string.Format("/v2/cryptocurrency/info?slug={0}", coin["slug"]));
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveColumnSet
                    {
                        Columns = new List<AdaptiveColumn>
                        {
                            new AdaptiveColumn
                            {
                                Width="3",
                                Spacing = AdaptiveSpacing.None,
                                Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveColumnSet
                                    {
                                        Columns = new List<AdaptiveColumn>
                                        {
                                            new AdaptiveColumn
                                            {
                                                Width = "3",
                                                Items = new List<AdaptiveElement>
                                                {
                                                    new AdaptiveImage
                                                    {
                                                        Url = new Uri(data["data"].First.First["logo"].ToString()),
                                                        Size = AdaptiveImageSize.Small
                                                    }
                                                }
                                            },
                                        }
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = data["data"].First.First["name"].ToString(),
                                        Weight = AdaptiveTextWeight.Bolder
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = Translator.Translate(data["data"].First.First["description"].ToString()).Result,
                                        Wrap = true,
                                        Spacing = AdaptiveSpacing.None
                                    },
                                }
                            }
                        }
                    },
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Title ="Xem thêm",
                        Url = new Uri("https://coinmarketcap.com/currencies/" + data["data"].First.First["slug"].ToString()),
                    }
                }
            };

            return card; 
        }
    }
}
