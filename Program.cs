using coinbase_bot;
using coinbase_bot.client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ICoinbaseClient, CoinbaseClient>();

Console.WriteLine("Starting the bot");
IHost host = builder.Build();
host.Run();
