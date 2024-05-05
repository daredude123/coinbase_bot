using coinbase_bot;
using coinbase_bot.client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ICoinbaseClient, CoinbaseClient>();

IHost host = builder.Build();
host.Run();
