using coinbase_bot.authorization;
using coinbase_bot.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace coinbase_bot.client
{
    public class CoinbaseClient(ILogger<CoinbaseClient> logger, IAuthorize authorization) : ICoinbaseClient
    {
        IAuthorize _authorize = authorization;
        private readonly ILogger<CoinbaseClient> _logger = logger;
        private static readonly HttpClient sharedClient =
            new() { BaseAddress = new Uri("https://api.coinbase.com") };

        //Get current price
        public async Task<BtcNokPrice> getCurrentPrice(string currencyPair)
        {
            string json = await CallCoinbase("/v2/prices/BTC-NOK/buy", HttpMethod.Get);
            _logger.LogInformation("response from coinbase {}", json);

            return JsonConvert.DeserializeObject<BtcNokPrice>(json);
        }

        private static async Task<string> CallCoinbase(string url, HttpMethod method)
        {
            HttpResponseMessage response = await sharedClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        private static BtcNokPrice Json2BtcNokPrice(string json)
        {
            BtcNokPrice obj = new();
            JObject ret = JObject.Parse(json);
            return obj;
        }


        private string getJWT()
        {

        }
    }
}
