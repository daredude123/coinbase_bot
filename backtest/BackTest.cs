using coinbase_bot.client;
using coinbase_bot.domain;
using Newtonsoft.Json.Linq;
using SimpleBacktestLib;
using Skender.Stock.Indicators;

namespace coinbase_bot.backtest;

public class BackTest(ICoinbaseClient publicClient) : IBackTest
{
    private readonly ICoinbaseClient _publicClient = publicClient;

    public async void RunBackTest()
    {
        var candleData = await getCandleData();

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

    private async Task<List<BacktestCandle>> getCandleData()
    {
        var priceList = await _publicClient.GetHistoricPrices("BTC-NOK");
        var quotes = Candle2Quote(priceList);
        return [];
    }

    private List<Quote> Candle2Quote(List<Candle> priceList)
    {
        List<Quote> qList = new List<Quote>();
        foreach (var price in priceList)
        {
            qList.Add(price2Quote(price));
        }
        return qList;
    }

    private Quote price2Quote(Candle candle)
    {
        Quote quote = new Quote
        {
            Low = candle.Low,
            High = candle.High,
            Volume = candle.Volume,
            Open = candle.Open,
            Date = candle.Start,

        };
    }
}
