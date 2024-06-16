using System.Net.Http.Headers;
using coinbase_bot.authorization;

namespace coinbase_bot.client;

public class PrivateCoinbaseClient(IAuthorize authorize, ILogger<PrivateCoinbaseClient> logger)
    : IPrivateCoinbaseClient
{
    private readonly IAuthorize _authorize = authorize;
    private readonly string OrgName = "algotraderkey";
    private readonly ILogger<PrivateCoinbaseClient> _logger = logger;

    private static readonly HttpClient sharedClient =
        new() { BaseAddress = new Uri("https://api.coinbase.com") };
    private string key;

    public async Task<string> ListProductsAsync()
    {
        return await CallCoinbase("api/v3/brokerage/products", HttpMethod.Get);
    }

    private async Task<string> CallCoinbase(string url, HttpMethod method)
    {
        string jwt = GetJWT(url, method);
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        sharedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            jwt
        );
        return await response.Content.ReadAsStringAsync();
    }

    private string GetJWT(string url, HttpMethod method)
    {
        Header header =
            new()
            {
                Alg = "ES256",
                Typ = "JWT",
                Kid = OrgName,
                Nonce = getEpoch()
            };

        string uri = method.Method + " " + url;

        Dictionary<string, object> claims =
            new()
            {
                { "iss", "cdp" },
                { "nbf", getEpoch() },
                { "exp", getEpoch(120) },
                { "sub", OrgName },
                { "uri", uri }
            };

        Payload payload = new Payload();
        payload.Claims = claims;
        string jwt = _authorize.MakeJwt(header, payload);

        return jwt;
    }

    private string getEpoch(int v = 0)
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        secondsSinceEpoch += v;
        return secondsSinceEpoch.ToString();
    }

    public string CreateOrder()
    {
        return "";
    }

    string IPrivateCoinbaseClient.ListAccountsAsync()
    {
        throw new NotImplementedException();
    }
}
