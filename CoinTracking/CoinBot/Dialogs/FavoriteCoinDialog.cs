using AdaptiveCards;
using CoinBot.Helpers;
using CoinBot.Models;
using CoinBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.Dialogs
{
    public class FavoriteCoinDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        private readonly Helpers.Common _common;
        private CoinMarketCapApi _coinMarketCapApi;

        #endregion

        public FavoriteCoinDialog(string dialogId, StateService stateService, CoinMarketCapApi coinMarketCapApi) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _common = new Helpers.Common();
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
            AddDialog(new WaterfallDialog($"{nameof(FavoriteCoinDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(FavoriteCoinDialog)}.name"));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(FavoriteCoinDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (userProfile.Id == null)
            {
                await stepContext.Context.SendActivityAsync("Vui lòng đăng nhập để sử dụng tính năng này", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            
            var card = GetFavoriteCoinCard(userProfile.Id);

            if(card==null)
            {
                await stepContext.Context.SendActivityAsync("Bạn chưa có coin yêu thích nào", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }    
            
            await stepContext.Context.SendActivityAsync(card, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private Activity GetFavoriteCoinCard(string userId)
        {
            List<Attachment> attachments = new();

            try
            {
                var response = _common.MakeAPICall(@"Coins/Favourite?userId=" + userId);

                var listCoints = response["items"].Children();

                foreach (var coin in listCoints)
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

                return bigCard;
            }
            catch
            {
                return null;
            }
        }
        public AdaptiveCard SendCard(JToken coin)
        {
            var data = _coinMarketCapApi.MakeAPICall(string.Format("/v2/cryptocurrency/info?symbol={0}", coin["symbol"]));
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
                                                        Url = new Uri(data["data"].First.First.First["logo"].ToString()),
                                                        Size = AdaptiveImageSize.Small
                                                    }
                                                }
                                            },
                                        }
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = data["data"].First.First.First["name"].ToString(),
                                        Weight = AdaptiveTextWeight.Bolder
                                    },
                                    new AdaptiveTextBlock
                                    {
                                        Text = Translator.Translate(data["data"].First.First.First["description"].ToString()).Result,
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
                        Url = new Uri("https://coinmarketcap.com/currencies/" + data["data"].First.First.First["slug"].ToString()),
                    }
                }
            };

            return card;
        }
    }
}