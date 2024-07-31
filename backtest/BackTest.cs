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
        List<Quote> quotes = [];

        bool position = false;
        decimal feePercentage = 0.6m;
        List<BacktestCandle> backTestCandleList = [];

        bool buystate = true;
        BacktestBuilder builder = BacktestBuilder
            .CreateBuilder(Candle2BackTestData(candleData))
            .AddSpotFee(AmountType.Percentage, feePercentage, FeeSource.Base)
            .WithQuoteBudget(1000)

            .OnTick(state =>
            {
                backTestCandleList.Add(state.GetCurrentCandle());

                Quote quote = BackTestCandle2Quote(state.GetCurrentCandle());
                quotes.Add(quote);

                var EMA_slow = quotes.GetEma(50).ToList();
                var EMA_fast = quotes.GetEma(30).ToList();
                var rsi = quotes.GetRsi(10);
                var bba_Bands = quotes.GetBollingerBands(15, 1.5);
                var atr = quotes.GetAtr(7);

                int emaFastStartIndex = Math.Max(0, (EMA_fast.Count - 1) - 7);
                int emaFastEndIndex = Math.Min(EMA_fast.Count, 7);

                int emaSlowStartIndex = Math.Max(0, (EMA_slow.Count - 1) - 7);
                int emaSlowEndIndex = Math.Min(EMA_slow.Count, 7);

                int emaSignal = 0;
                if (CheckEmaTrend(EMA_fast.Slice(emaFastStartIndex, emaFastEndIndex), EMA_slow.Slice(emaSlowStartIndex, emaSlowEndIndex)))
                {
                    emaSignal = 1;
                }
                else if (CheckEmaTrend(EMA_slow.Slice(emaSlowStartIndex, emaSlowEndIndex), EMA_fast.Slice(emaFastStartIndex, emaFastEndIndex)))
                {
                    emaSignal = 2;
                }

                if (emaSignal == 2 && state.GetCurrentCandle().Close <= (decimal?)bba_Bands.Last().LowerBand)//&& rsi.Last().Rsi < 60 && buystate)
                {
                    state.Trade.Spot.Buy();
                    buystate = false;
                }
                if (emaSignal == 1 && state.GetCurrentCandle().Close >= (decimal?)bba_Bands.Last().UpperBand)//&& rsi.Last().Rsi > 40 && !buystate)
                {
                    state.Trade.Spot.Sell();
                    buystate = true;
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

        return await _publicClient.GetHistoricPricesInBatch("BTC-USD", 10);
    }

    private static bool CheckEmaTrend(List<EmaResult> firstList, List<EmaResult> lastList)
    {

        for (int i = 0; i < firstList.Count; i++)
        {
            if (firstList[i].Ema < lastList[i].Ema)
            {
                return false;
            }
        }
        return true;
    }

    private static int CalculateTrend(List<BacktestCandle> backtestCandles, List<VwapResult> vwaps)
    {

        int backCandles = 15;
        for (int i = backCandles; i < backtestCandles.Count; i++)
        {
            int upt = 1;
            int dnt = 1;
            for (int j = i - backCandles; j < i + 1; j++)
            {

                if (Math.Max(backtestCandles[j].Open, backtestCandles[j].Close) >= ((decimal?)vwaps[j].Vwap))
                {
                    dnt = 0;
                }
                if (Math.Max(backtestCandles[j].Open, backtestCandles[j].Close) <= ((decimal?)vwaps[j].Vwap))
                {
                    upt = 0;
                }
                if (upt == 1 && dnt == 1)
                {
                    return 3;
                }
                else if (upt == 1)
                {
                    return 2;
                }
                else if (dnt == 1)
                {
                    return 1;
                }
            }
        }
        return 0;
    }



    private static List<BacktestCandle> Candle2BackTestData(HistoricalCandles historicalCandles)
    {
        List<BacktestCandle> qList = [];
        /*historicalCandles.candles.Reverse();*/
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
