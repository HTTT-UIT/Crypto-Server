using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CoinBot.Helpers
{
    public class CoinMarketCapApi
    {
        private static string apiKey;
        private static string baseUrl;
        public CoinMarketCapApi(IConfiguration configuration)
        {
            apiKey =  configuration["CoinMarketCapApiKey"];
            baseUrl = configuration["CoinMarketCapBaseUrl"];
        }
        
        public JObject MakeAPICall(string url)
        {
            var URL = new UriBuilder(baseUrl + url);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
            client.DefaultRequestHeaders.Add("Accepts", "application/json");
            var result = JObject.Parse((client.GetStringAsync(URL.ToString()).Result));
            return result;
        }
    }
}
