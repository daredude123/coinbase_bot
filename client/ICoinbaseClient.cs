using coinbase_bot.domain;

namespace coinbase_bot.client;

public interface ICoinbaseClient
{
    Task<BtcNokPrice> GetCurrentPrice(string currencyPair);
    Task<List<BtcNokPrice>> GetHistoricPrices(string pricePair);
}
