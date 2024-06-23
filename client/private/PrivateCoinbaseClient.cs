using System.Net.Http.Headers;
using System.Security.Cryptography;
using coinbase_bot.authorization;
using Jose;

namespace coinbase_bot.client;

public class PrivateCoinbaseClient(IAuthorize authorize, ILogger<PrivateCoinbaseClient> logger)
    : IPrivateCoinbaseClient
{
    static Random random = new Random();
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
        string key = parseKey(cbPrivateKey);
        string endpoint = "api.coinbase.com/api/v3/brokerage/accounts";
        string token = generateToken(name, key, $"GET {endpoint}");

        Console.WriteLine("Call API...");
        Console.WriteLine(CallApiGET($"https://{endpoint}", token));

        HttpResponseMessage response = await sharedClient.GetAsync(url);
        sharedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            jwt
        );

        _logger.LogInformation(response.ToString());
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

    static string generateToken(string name, string secret, string uri)
    {
        var privateKeyBytes = Convert.FromBase64String(secret); // Assuming PEM is base64 encoded
        using var key = ECDsa.Create();
        key.ImportECPrivateKey(privateKeyBytes, out _);

        var payload = new Dictionary<string, object>
        {
            { "sub", name },
            { "iss", "coinbase-cloud" },
            {
                "nbf",
                Convert.ToInt64(
                    (
                        DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    ).TotalSeconds
                )
            },
            {
                "exp",
                Convert.ToInt64(
                    (
                        DateTime.UtcNow.AddMinutes(1)
                        - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    ).TotalSeconds
                )
            },
            { "uri", uri }
        };

        var extraHeaders = new Dictionary<string, object>
        {
            { "kid", name },
            // add nonce to prevent replay attacks with a random 10 digit number
            { "nonce", randomHex(10) },
            { "typ", "JWT" }
        };

        var encodedToken = JWT.Encode(payload, key, JwsAlgorithm.ES256, extraHeaders);

        // print token
        Console.WriteLine(encodedToken);
        return encodedToken;
    }

    static string parseKey(string key)
    {
        List<string> keyLines = new List<string>();
        keyLines.AddRange(key.Split('\n', StringSplitOptions.RemoveEmptyEntries));

        keyLines.RemoveAt(0);
        keyLines.RemoveAt(keyLines.Count - 1);

        return String.Join("", keyLines);
    }

    static string randomHex(int digits)
    {
        byte[] buffer = new byte[digits / 2];
        random.NextBytes(buffer);
        string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
        if (digits % 2 == 0)
            return result;
        return result + random.Next(16).ToString("X");
    }

    static string CallApiGET(string url, string bearerToken = "")
    {
        using (var client = new HttpClient())
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (bearerToken != "")
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Bearer",
                            bearerToken
                        );
                var response = client.Send(request);

                if (response != null)
                    return response.Content.ReadAsStringAsync().Result;
                else
                    return "";
            }
        }
    }
}
