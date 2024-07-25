using coinbase_bot.domain;

namespace coinbase_bot.client;

public interface ICoinbaseClient
{
    Task<BtcNokPrice> GetCurrentPrice(string currencyPair);
    Task<HistoricalCandles> GetHistoricPrices(string pricePair);
    Task<HistoricalCandles> GetHistoricPricesInBatch(string pricePair, DateTime start, DateTime end);
}
