using coinbase_bot.domain;

namespace coinbase_bot.client;

public interface ICoinbaseClient
{
    Task<BtcNokPrice> getCurrentPrice(string currencyPair);
}
