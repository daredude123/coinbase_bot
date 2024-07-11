using coinbase_bot.client;
using coinbase_bot.domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SimpleBacktestLib;
using Skender.Stock.Indicators;

namespace coinbase_bot.backtest;

public class BackTest(ICoinbaseClient publicClient) : IBackTest
{
    private readonly ICoinbaseClient _publicClient = publicClient;

    public async void RunBackTest()
    {
        var candleData = await GetCandleData();
        var strategyData = Candle2Quote(await GetCandleData());

        BacktestBuilder builder = BacktestBuilder
            .CreateBuilder(Candle2BackTestData(candleData))
            .OnTick(state =>
            {
                state.Trade.Spot.Buy();
            })
            .OnLogEntry(
                (logEntry, state) => {
                    // Optionally do something when a log entry is created
                }
            );

        BacktestResult result = await builder.RunAsync();
        var resultString = JsonConvert.SerializeObject(
            result,
            Formatting.Indented,
            [new StringEnumConverter()]
        );
        Console.WriteLine(resultString);
    }

    private async Task<HistoricalCandles> GetCandleData()
    {
        return await _publicClient.GetHistoricPrices("BTC-USD");
    }

    private static List<BacktestCandle> Candle2BackTestData(HistoricalCandles historicalCandles)
    {
        List<BacktestCandle> qList = [];
        historicalCandles.candles.Reverse();
        foreach (Candle price in historicalCandles.candles)
        {
            BacktestCandle ret = ToBackTestCandles(price);
            Console.WriteLine(ret.ToString());
            qList.Add(ret);

        }

        return qList;
    }

    private static BacktestCandle ToBackTestCandles(Candle candle)
    {
        return new BacktestCandle
        {
            Low = candle.Low,
            High = candle.High,
            Volume = candle.Volume,
            Open = candle.Open,
            Time = DateTimeOffset.FromUnixTimeSeconds(candle.Start).DateTime
        };
    }

    private static List<Quote> Candle2Quote(HistoricalCandles historicalCandles)
    {
        List<Quote> qList = [];
        foreach (Candle price in historicalCandles.candles)
        {
            qList.Add(Price2Quote(price));
        }
        return qList;
    }

    private static Quote Price2Quote(Candle candle)
    {
        return new Quote
        {
            Low = candle.Low,
            High = candle.High,
            Volume = candle.Volume,
            Open = candle.Open,
            Date = DateTimeOffset.FromUnixTimeSeconds(candle.Start).UtcDateTime
        };
    }
}
