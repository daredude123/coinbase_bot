using System.Net;
using coinbase_bot.domain;
using Newtonsoft.Json;

namespace coinbase_bot.client
{
    public class CoinbaseClient(ILogger<CoinbaseClient> logger) : ICoinbaseClient
    {
        private readonly ILogger<CoinbaseClient> _logger = logger;
        private static readonly HttpClient sharedClient =
            new() { BaseAddress = new Uri("https://api.coinbase.com") };

        //Get current price
        public async Task<BtcNokPrice> GetCurrentPrice(string currencyPair)
        {
            string json = await CallCoinbase("/v2/prices/BTC-NOK/buy");
            return JsonConvert.DeserializeObject<BtcNokPrice>(json);
        }

        private static async Task<string> CallCoinbase(string url)
        {
            HttpResponseMessage response = await sharedClient.GetAsync(url);
            return response.StatusCode != HttpStatusCode.OK
                ? throw new Exception(await response.Content.ReadAsStringAsync())
                : await response.Content.ReadAsStringAsync();
        }

        private static BtcNokPrice Json2BtcNokPrice(string json)
        {
            BtcNokPrice btcNokPrice = JsonConvert.DeserializeObject<BtcNokPrice>(json);
            Console.WriteLine(value: btcNokPrice.Data.Amount);
            return btcNokPrice;
        }

        public async Task<HistoricalCandles> GetHistoricPrices(string pricePair)
        {
            //Current date in Unix seconds
            long end = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            long start = new DateTimeOffset(DateTime.Now).AddHours(-20).ToUnixTimeSeconds();
            string json = await CallCoinbase(
                $"api/v3/brokerage/market/products/{pricePair}/candles?start={start}&end={end}&granularity=FIVE_MINUTE"
            );

            return JsonConvert.DeserializeObject<HistoricalCandles>(json);
        }

        public Task<HistoricalCandles> GetHistoricPricesInBatch(string pricePair, DateTime start, DateTime end)
        {

            if(DateTime.Now.Subtract(start).Days > 20)
            {
                //we need to divy up

            }
            //Current date in Unix seconds
            long longEnd = new DateTimeOffset(end).ToUnixTimeSeconds();
            long longStart = new DateTimeOffset(start).ToUnixTimeSeconds();


        }
    }
}
