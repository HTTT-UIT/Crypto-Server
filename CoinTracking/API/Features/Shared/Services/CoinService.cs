using API.Features.Coins.Commands;
using MediatR;
using Newtonsoft.Json;

namespace API.Features.Shared.Services
{
    public class CoinService : ICoinService
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        
        private readonly string _baseCoinUrl = "https://pro-api.coinmarketcap.com";

        public CoinService(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public Guid SyncCoin(string coinId)
        {
            var getCoinDetailPath = $"/v2/cryptocurrency/info?id={coinId}";

            using HttpClient client = new();

            var api_key = _configuration["COIN_API_KEY"];
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", api_key);
            var result = client.GetFromJsonAsync<SyncCoin>(_baseCoinUrl + getCoinDetailPath).Result;

            if (result!=null && result.Status.Error_code == 0 && result.Data.Any())
            {
                Create.Command command = new()
                {
                    Request = new()
                };
                command.Request.Name = result.Data.First().Value.Name;
                command.Request.RefId = result.Data.First().Value.Id.ToString();
                command.Request.Symbol = result.Data.First().Value.Symbol;

                var newCoin = _mediator.Send(command).Result;

                return newCoin.Value?.Id ?? Guid.Empty; 
            }

            return Guid.Empty;

        }
    }

    public class SyncCoin
    {
        public Status Status { get; set; } = new();

        [JsonProperty("data")]
        public Dictionary<int, Data> Data { get; set; } = new();
    }

    public class Status
    {
        public string Error_message { get; set; } = string.Empty;
        public int Error_code { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Symbol { get; set; } = string.Empty;
    }
}
