using coinbase_bot;
using coinbase_bot.authorization;
using coinbase_bot.client;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ICoinbaseClient, CoinbaseClient>();
builder.Services.AddSingleton<IAuthorize, Authorize>();
builder.Services.AddSingleton<IPrivateCoinbaseClient, PrivateCoinbaseClient>();
Console.WriteLine("Starting the bot");
IHost host = builder.Build();
host.Run();
