using coinbase_bot.client;

namespace coinbase_bot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private ICoinbaseClient _client;

    public Worker(ILogger<Worker> logger, ICoinbaseClient client)
    {
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var price = await _client.getCurrentPrice("BTC-NOK");
            _logger.LogInformation(price._amount + "");
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
