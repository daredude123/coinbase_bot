using System.Collections.Generic;
using coinbase_bot.authorization;

namespace coinbase_bot.client;

public class PrivateCoinbaseClient(IAuthorize authorize, ILogger<PrivateCoinbaseClient> logger) : IPrivateCoinbaseClient
{
    private readonly IAuthorize _authorize = authorize;
    private readonly string OrgName;
    private readonly ILogger<PrivateCoinbaseClient> _logger = logger;

    private static readonly HttpClient sharedClient =
        new() { BaseAddress = new Uri("https://api.coinbase.com") };
    private string key;

    public string ListAccounts()
    {


    }


    private static async Task<string> CallCoinbase(string url, HttpMethod method)
    {
        string jwt = getJWT(url);
        HttpResponseMessage response = await sharedClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private string getJWT(string url)
    {
        Header header = new Header();
        header.Alg = "ES256";
        header.Typ = "JWT";
        header.Kid = OrgName;
        header.Nonce = getEpoch();


        Dictionary<string, object> claims = new Dictionary<string,object>();
        Payload payload = new Payload();
        payload.Claims = claims;
       string jwt = _authorize.MakeJwt(header, payload, secretKey: key);

        return jwt;
    }

    private string getEpoch()
    {
        TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
        int secondsSinceEpoch = (int)t.TotalSeconds;
        return secondsSinceEpoch.ToString();
    }
}
