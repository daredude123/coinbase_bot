namespace coinbase_bot.client;

public interface IPrivateCoinbaseClient
{
    public string ListAccountsAsync();
    public Task<string> ListProductsAsync();
}
