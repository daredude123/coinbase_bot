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
        List<BacktestCandle> backTestCandleList = [];

        BacktestBuilder builder = BacktestBuilder
            .CreateBuilder(Candle2BackTestData(candleData))
            .AddSpotFee(AmountType.Percentage, feePercentage, FeeSource.Base)
            .OnTick(state =>
            {
                backTestCandleList.Add(state.GetCurrentCandle());

                Quote quote = BackTestCandle2Quote(state.GetCurrentCandle());
                quotes.Add(quote);
                Console.WriteLine($"'{state.GetCurrentCandle().High}' '{state.GetCurrentCandle().Low}' '{state.GetCurrentCandle().Open}' '{state.GetCurrentCandle().Time}' '{state.GetCurrentCandle().Volume}' '{state.GetCurrentCandle().Close}'");

                IEnumerable<SmaResult> sma20 = quotes.GetSma(20);
                IEnumerable<BollingerBandsResult> bollingerB = quotes.GetBollingerBands(15);
                IEnumerable<VwapResult> vWap = quotes.GetVwap();
                IEnumerable<RsiResult> rsi = quotes.GetRsi(16);

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

    private int calculateTrend(List<BacktestCandle> )



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
