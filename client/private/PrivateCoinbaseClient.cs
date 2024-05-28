using coinbase_bot.authorization;

namespace coinbase_bot.client;

public class PrivateCoinbaseClient(IAuthorize authorize, ILogger<PrivateCoinbaseClient> logger)
    : IPrivateCoinbaseClient
{
    private readonly IAuthorize _authorize = authorize;
    private readonly string OrgName;
    private readonly ILogger<PrivateCoinbaseClient> _logger = logger;

    private static readonly HttpClient sharedClient =
        new() { BaseAddress = new Uri("https://api.coinbase.com") };
    private string key;

    public string ListAccounts()
    {
        return "";
    }

    private async Task<string> CallCoinbase(string url, HttpMethod method)
    {
        string jwt = getJWT(url, method);
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private string getJWT(string url, HttpMethod method)
    {
        Header header = new Header();
        header.Alg = "ES256";
        header.Typ = "JWT";
        header.Kid = OrgName;
        header.Nonce = getEpoch();

        string uri  = method.Method + " " + url;

        Dictionary<string, object> claims = new Dictionary<string, object>();
        claims.Add("iss", "cdp");
        claims.Add("nbf", getEpoch());
        claims.Add("exp", getEpoch(120));
        claims.Add("sub", OrgName);
        claims.Add("uri", uri);

        Payload payload = new Payload();
        payload.Claims = claims;
        string jwt = _authorize.MakeJwt(header, payload, secretKey: key);

        return jwt;
    }

    private string getEpoch(int v = 0)
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        secondsSinceEpoch += v;
        return secondsSinceEpoch.ToString();
    }
}
