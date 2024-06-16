using coinbase_bot.client;
using coinbase_bot.domain;

namespace coinbase_bot;

public class Worker(ILogger<Worker> logger, ICoinbaseClient client, IPrivateCoinbaseClient privateCoinbaseClient) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly ICoinbaseClient _client = client;
    private readonly IPrivateCoinbaseClient _privateClient = privateCoinbaseClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            BtcNokPrice price = await _client.getCurrentPrice("BTC-NOK");
            _logger.LogInformation(price.Data.Amount + "");

            var products = await _privateClient.ListProductsAsync();
            _logger.LogInformation(products);


            await Task.Delay(1000, stoppingToken);
        }
    }
}
