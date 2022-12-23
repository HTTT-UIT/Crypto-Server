using API.Features.Coins.Commands;
using MediatR;
using Newtonsoft.Json;

namespace API.Features.Shared.Services
{
    public class CoinService : ICoinService
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        
        private readonly string _baseCoinUrl = "https://api.coincap.io/v2/assets/";

        public CoinService(IConfiguration configuration, IMediator mediator)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public Guid SyncCoin(string coinId)
        {
            using HttpClient client = new();

            var result = client.GetFromJsonAsync<SyncCoin>(_baseCoinUrl + coinId).Result;

            if (result != null)
            {
                Create.Command command = new()
                {
                    Request = new()
                };
                command.Request.Name = result.Data.Name;
                command.Request.RefId = result.Data.Id;
                command.Request.Symbol = result.Data.Symbol;

                var newCoin = _mediator.Send(command).Result;

                return newCoin.Value?.Id ?? Guid.Empty;
            }

            return Guid.Empty;

        }
    }

    public class SyncCoin
    {
        [JsonProperty("data")]
        public Data Data { get; set; } = new();
    }

    public class Data
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Symbol { get; set; } = string.Empty;
    }
}
