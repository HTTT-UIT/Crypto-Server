using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net.Http;

namespace CoinBot.Helpers
{
    public class Common
    {
        private readonly string baseUrl = @"http://localhost:9091/api/";
        public JObject MakeAPICall(string url)
        {
            var URL = new UriBuilder(baseUrl + url);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accepts", "application/json");
            var result = JObject.Parse((client.GetStringAsync(URL.ToString()).Result));
            return result;
        }

        public JObject MakeCoinCapApiCall(string url)
        {
            var URL = new UriBuilder(@"https:\\api.coincap.io\v2\" + url);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accepts", "application/json");
            var result = JObject.Parse((client.GetStringAsync(URL.ToString()).Result));
            return result;
        }    
    }

    
}
