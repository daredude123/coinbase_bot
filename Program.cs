using coinbase_bot;
using coinbase_bot.authorization;
using coinbase_bot.backtest;
using coinbase_bot.client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ICoinbaseClient, CoinbaseClient>();
builder.Services.AddSingleton<IAuthorize, Authorize>();
builder.Services.AddSingleton<IPrivateCoinbaseClient, PrivateCoinbaseClient>();
builder.Services.AddSingleton<IBackTest, BackTest>();

Console.WriteLine("Starting the bot");
IHost host = builder.Build();
host.Run();
