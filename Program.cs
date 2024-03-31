using coinbase_bot;
using coinbase_bot.client;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddHostedService<CoinbaseClient>();

        var host = builder.Build();
        host.Run();
    }
}
