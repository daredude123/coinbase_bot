using coinbase_bot.backtest;
using coinbase_bot.client;
using coinbase_bot.domain;

namespace coinbase_bot;

public class Worker(
    ILogger<Worker> logger,
    ICoinbaseClient client,
    IPrivateCoinbaseClient privateCoinbaseClient,
    IBackTest backTest
) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly ICoinbaseClient _client = client;
    private readonly IPrivateCoinbaseClient _privateClient = privateCoinbaseClient;
    private readonly bool backtestFlag = true;
    private readonly IBackTest _backTest = backTest;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (backtestFlag)
        {
            _backTest.RunBackTest();
        }
        else
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                BtcNokPrice price = await _client.GetCurrentPrice("BTC-NOK");
                _logger.LogInformation(price.Data.Amount + "");

                var products = await _privateClient.ListProductsAsync();
                _logger.LogInformation(products.ToString());

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
