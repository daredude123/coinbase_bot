using coinbase_bot.client;
using coinbase_bot.domain;

namespace coinbase_bot;

public class Worker(ILogger<Worker> logger, ICoinbaseClient client) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly ICoinbaseClient _client = client;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            BtcNokPrice price = await _client.getCurrentPrice("BTC-NOK");
            _logger.LogInformation(message: price.Data.Amount + "");
            await Task.Delay(1000, stoppingToken);
        }
    }
}
