using coinbase_bot.authorization;

namespace coinbase_bot.client;

public class PrivateCoinbaseClient(IAuthorize authorize, ILogger<PrivateCoinbaseClient> logger) : IPrivateCoinbaseClient
{
    private readonly string coinbaseApi = "https://api.coinbase.com";
    private readonly IAuthorize _authorize = authorize;
    private readonly string OrgName;
    private readonly ILogger<PrivateCoinbaseClient> _logger = logger;

    private static readonly HttpClient sharedClient =
        new() { BaseAddress = new Uri("https://api.coinbase.com") };

    public string ListAccounts()
    {
        return "";
    }


    private static async Task<string> CallCoinbase(string url, HttpMethod method)
    {
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}
