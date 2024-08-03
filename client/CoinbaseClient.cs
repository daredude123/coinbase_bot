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

        public async Task<HistoricalCandles> GetHistoricPricesInBatch(string pricePair, int periods)
        {

            HistoricalCandles candles = new()
            {
                candles = []
            };

            if (periods == 0)
            {
                long longStart = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
                long longEnd = new DateTimeOffset(DateTime.Now).AddHours(-20).ToUnixTimeSeconds();

                string json = await CallCoinbase(
                    $"api/v3/brokerage/market/products/{pricePair}/candles?start={longStart}&end={longEnd}&granularity=FIVE_MINUTE"
                );

                HistoricalCandles ret = JsonConvert.DeserializeObject<HistoricalCandles>(json);
                candles.candles.AddRange(ret.candles);
            }
            else
            {
                for (int i = periods; i > 0; i--)
                {
                    Console.WriteLine("Collecting batch nr. " + i);
                    int offsetStart = -(i * 20);
                    int offsetEnd = offsetStart + 20;

                    long longStart = new DateTimeOffset(DateTime.Now).AddHours(offsetStart).ToUnixTimeSeconds();
                   long longEnd = new DateTimeOffset(DateTime.Now).AddHours(offsetEnd).ToUnixTimeSeconds();

                    string json = await CallCoinbase($"api/v3/brokerage/market/products/{pricePair}/candles?start={longStart}&end={longEnd}&granularity=FIVE_MINUTE");
                    HistoricalCandles ret = JsonConvert.DeserializeObject<HistoricalCandles>(json);

                    ret.candles.Reverse();
                    candles.candles.AddRange(ret.candles);
                }
            }
            return candles;
        }
    }
}
