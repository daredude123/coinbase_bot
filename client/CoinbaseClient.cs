using coinbase_bot.domain;
using Newtonsoft.Json;

namespace coinbase_bot.client;

public class CoinbaseClient
{

    string coinbaseClient = "https://api.coinbase.com";//"/v2/prices/BTC-USD/buy";
    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri("https://api.coinbase.com")
    };

    public async Task<BtcNokPrice> getCurrentPrice(string currencyPair)
    {
        string json = await callCoinbase("/v2/prices/BTC-NOK/BUY", HttpMethod.Get);
        return JsonConvert.DeserializeObject<BtcNokPrice>(json);
    }

    public async Task<string> callCoinbase(string url, HttpMethod method)
    {
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();

    }
}
