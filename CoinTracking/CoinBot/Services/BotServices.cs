using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System;

namespace CoinBot.Services
{
    public class BotServices
    {
        public BotServices(IConfiguration configuration)
        {
            // Read the .bot file and grab the services from it
            var luisApplication = new LuisApplication(
                configuration["DispatchAppId"],
                configuration["DispatchAPIKey"],
                $"https://{configuration["DispatchAPIHostName"]}.api.cognitive.microsoft.com");

            var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
            {
                PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true,
                    Slot = configuration["DispatchSlot"]
                }
            };

            Dispatch = new LuisRecognizer(recognizerOptions);
        }

        public LuisRecognizer Dispatch { get; private set;}
    }
}
