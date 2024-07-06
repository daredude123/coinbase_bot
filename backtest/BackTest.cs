using coinbase_bot.client;
using SimpleBacktestLib;

namespace coinbase_bot.backtest;

public class BackTest(ICoinbaseClient publicClient) : IBackTest
{
    private readonly ICoinbaseClient _publicClient = publicClient;

    public void RunBackTest()
    {
        var candleData = new List<BacktestCandle>() { };

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

    private List<BacktestCandle> getCandleData()
    {
        var priceList = _publicClient.GetHistoricPrices("BTC-NOK");
        return [];
    }
}
