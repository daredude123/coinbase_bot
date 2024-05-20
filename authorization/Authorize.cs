using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace coinbase_bot.authorization;

public class Authorize : IAuthorize
{
    private readonly string keyName = "";
    private readonly string keySecret = "";

    public string MakeJwt(Header header, Payload payload, string secretKey)
    {
        string headerJson = SerializeObject(header);
        string payloadJson = SerializeObject(payload.Claims);

        string base64Header = Base64UrlEncode(headerJson);
        string base64Payload = Base64UrlEncode(payloadJson);

        string signature = GenerateSignature(base64Header, base64Payload, secretKey);
        return $"{base64Header}.{base64Payload}.{signature}";
    }

    static DefaultContractResolver contractResolver = new DefaultContractResolver
    {
        NamingStrategy = new CamelCaseNamingStrategy()
    };

    private static string SerializeObject(object obj)
    {
        return JsonConvert.SerializeObject(
            obj,
            new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            }
        );
    }

    public static string Base64UrlEncode(string param)
    {
        var bytes = Encoding.UTF8.GetBytes(param);
        return Base64UrlEncode(bytes);
    }

    public static string Base64UrlEncode(byte[] bytes)
    {
        var base64 = System.Convert.ToBase64String(bytes);
        var base64Url = base64.TrimEnd('=').Replace('+', '-').Replace('/', '-');
        return base64Url;
    }

    private static string GenerateSignature(
        string base64Header,
        string base64Payload,
        string secretKey
    )
    {
        var cipherText = $"{base64Header}.{base64Payload}";
        HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hashResult = hmac.ComputeHash(Encoding.UTF8.GetBytes(cipherText));
        return Base64UrlEncode(hashResult);
    }
}
