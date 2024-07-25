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
        HistoricalCandles candleData = await GetCandleData();

        List<Quote> strategyData = Candle2Quote(candleData);
        Console.WriteLine(strategyData.Count);
        List<Quote> quotes = [];

        bool position = false;
        decimal feePercentage = 0.6m;

        BacktestBuilder builder = BacktestBuilder
            .CreateBuilder(Candle2BackTestData(candleData))
            .AddSpotFee(AmountType.Percentage, feePercentage, FeeSource.Base)
            .OnTick(state =>
            {

                Quote quote = BackTestCandle2Quote(state.GetCurrentCandle());
                quotes.Add(quote);
                Console.WriteLine($"'{state.GetCurrentCandle().High}' '{state.GetCurrentCandle().Low}' '{state.GetCurrentCandle().Open}' '{state.GetCurrentCandle().Time}' '{state.GetCurrentCandle().Volume}' '{state.GetCurrentCandle().Close}'");

                IEnumerable<SmaResult> sma20 = quotes.GetSma(20);
                quotes.GetBollingerBands(15);
                quotes.GetVwap();
                IEnumerable<SmaResult> sma40 = quotes.GetSma(40);
                SmaResult last20 = new(DateTime.Now);
                SmaResult last40 = new(DateTime.Now);
                if (sma20.Any())
                {
                    last20 = sma20.Last();
                }
                if (sma40.Any())
                {
                    last40 = sma40.Last();
                }
                if (last20.Sma > last40.Sma && !position)
                {
                    position = true;
                    state.Trade.Spot.Buy(AmountType.Percentage, 10);
                }
                else if (last20.Sma < last40.Sma && position)
                {
                    position = false;
                    state.Trade.Spot.Sell(AmountType.Percentage, 100);
                }
            })
            .OnLogEntry(
                (logEntry, state) =>
                {
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
            qList.Add(ret);
        }
        return qList;
    }

    private static BacktestCandle ToBackTestCandles(Candle candle)
    {
        return new BacktestCandle
        {
            Low = candle.Low,
            Close = candle.Close,
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
    private static Quote BackTestCandle2Quote(BacktestCandle candle)
    {
        return new Quote
        {
            Close = candle.Close,
            Low = candle.Low,
            High = candle.High,
            Volume = candle.Volume,
            Open = candle.Open,
            Date = candle.Time
        };
    }
}
