using coinbase_bot.client;
using coinbase_bot.domain;
using Newtonsoft.Json.Linq;
using SimpleBacktestLib;
using Skender.Stock.Indicators;

namespace coinbase_bot.backtest;

public class BackTest(ICoinbaseClient publicClient) : IBackTest
{
    private readonly ICoinbaseClient _publicClient = publicClient;

    public void RunBackTest()
    {
        var candleData = getCandleData();

        BacktestBuilder builder = BacktestBuilder
            .CreateBuilder(candleData)
            .OnTick(state =>
            {

                // your strategy code goes here
            })
            .OnLogEntry(
                (logEntry, state) => {
                    // Optionally do something when a log entry is created
                }
            );

        builder.Run();
    }

    private async List<BacktestCandle> getCandleData()
    {
        var priceList = await _publicClient.GetHistoricPrices("BTC-NOK");
        JObject jsonObject = JObject.Parse(priceList);
        var quotes = prices2Quotes(priceList);
        return [];
    }

    private object prices2Quotes(List<BtcNokPrice> priceList)
    {
        List<Quote> qList = new List<Quote>();
        foreach (var price in priceList)
        {
            qList.Add(price2Quote(price));
        }
    }

    private Quote price2Quote(Candle price)
    {
        Candlesticks
    }
}
