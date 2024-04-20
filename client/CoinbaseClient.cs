using coinbase_bot.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace coinbase_bot.client;

public class CoinbaseClient : ICoinbaseClient
{
    private readonly ILogger<CoinbaseClient> _logger;
    private static HttpClient sharedClient =
        new() { BaseAddress = new Uri("https://api.coinbase.com") };

    public CoinbaseClient(ILogger<CoinbaseClient> logger)
    {
        _logger = logger;
    }

    public async Task<BtcNokPrice> getCurrentPrice(string currencyPair)
    {
        string json = await callCoinbase("/v2/prices/BTC-NOK/buy", HttpMethod.Get);
        _logger.LogInformation(json);
        BtcNokPrice test = json2BtcNokPrice(json);
        return JsonConvert.DeserializeObject<BtcNokPrice>(json);
    }

    private async Task<string> callCoinbase(string url, HttpMethod method)
    {
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private BtcNokPrice json2BtcNokPrice(string json)
    {
        BtcNokPrice obj = new BtcNokPrice();
        dynamic ret = JObject.Parse(json);
        return obj;
    }
}
