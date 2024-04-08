using coinbase_bot.domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace coinbase_bot.client;

public class CoinbaseClient : ICoinbaseClient
{

    private readonly ILogger _logger;
    string coinbaseClientUrl = "https://api.coinbase.com";//"/v2/prices/BTC-USD/buy";
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://api.coinbase.com")
    };

    public CoinbaseClient()
    {
    }

    public async Task<BtcNokPrice> getCurrentPrice(string currencyPair)
    {
        string json = await callCoinbase("/v2/prices/BTC-NOK/buy", HttpMethod.Get);
        Console.WriteLine(json);

        return JsonConvert.DeserializeObject<BtcNokPrice>(json);
    }

    private async Task<string> callCoinbase(string url, HttpMethod method)
    {
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        Console.WriteLine("HELLO", response.StatusCode);
        return await response.Content.ReadAsStringAsync();

    }
}
