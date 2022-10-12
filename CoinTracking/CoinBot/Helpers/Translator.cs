using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Helpers
{
    public static class Translator
    {
        private static readonly string key = "fc52545229ac41699c2d2228a38bea12";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
        private static readonly string region = "eastus";

        public static async Task<string> Translate(string textToTranslate)
        {
            // Input and output languages are defined as parameters.
            string route = "/translate?api-version=3.0&to=vi";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(endpoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", key);
                request.Headers.Add("Ocp-Apim-Subscription-Region", region);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                var result = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(result.Remove(result.Length - 1, 1).Remove(0, 1));

                return obj["translations"][0]["text"].ToString();
            }

        }
    }
}
