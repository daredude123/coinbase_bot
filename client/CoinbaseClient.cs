using coinbase_bot.authorization;
using coinbase_bot.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace coinbase_bot.client
{
    public class CoinbaseClient(ILogger<CoinbaseClient> logger, IAuthorize authorize)
        : ICoinbaseClient
    {
        string coinbaseApi = "https://api.coinbase.com";
        IAuthorize _authorize = authorize;
        private string OrgName;

        private readonly ILogger<CoinbaseClient> _logger = logger;
        private static readonly HttpClient sharedClient =
            new() { BaseAddress = new Uri("https://api.coinbase.com") };

        //Get current price
        public async Task<BtcNokPrice> getCurrentPrice(string currencyPair)
        {
            string uri = coinbaseApi + "/api/v3/brokerage/accounts";
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

        private string GetJWT(HttpMethod method, string uri)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            Header header = new Header
            {
                Alg = "ES256",
                Typ = "JWT",
                Nonce = secondsSinceEpoch.ToString(),
                Kid = OrgName
            };

            var claims = new Dictionary<string, object>();
            claims.Add("iss", "cdp");
            claims.Add("nbf", secondsSinceEpoch.ToString());
            claims.Add("exp", secondsSinceEpoch + 120);
            claims.Add("sub", OrgName);
            claims.Add("uri", uri);

            Payload payload = new Payload { Claims = claims };

            return _authorize.MakeJwt(header, payload, "quack");
        }
    }
}
