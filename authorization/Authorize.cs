using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace coinbase_bot.authorization;

public class Authorize : IAuthorize
{
    private readonly string keyName = "algotraderkey";
    private readonly string keySecret = "TFeY1BHxPH5rcin5";

    public string MakeJwt(Header header, Payload payload)
    {
        string headerJson = SerializeObject(header);
        string payloadJson = SerializeObject(obj: payload.Claims);

        string base64Header = Base64UrlEncode(headerJson);
        string base64Payload = Base64UrlEncode(payloadJson);

        string signature = GenerateSignature(base64Header, base64Payload, keySecret);
        return $"{base64Header}.{base64Payload}.{signature}";
    }

    private static readonly DefaultContractResolver contractResolver = new()
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
        byte[] bytes = Encoding.UTF8.GetBytes(param);
        return Base64UrlEncode(bytes);
    }

    public static string Base64UrlEncode(byte[] bytes)
    {
        string base64 = Convert.ToBase64String(bytes);
        string base64Url = base64.TrimEnd('=').Replace('+', '-').Replace('/', '-');
        return base64Url;
    }

    private static string GenerateSignature(
        string base64Header,
        string base64Payload,
        string secretKey
    )
    {
        string cipherText = $"{base64Header}.{base64Payload}";
        HMACSHA256 hmac = new(Encoding.UTF8.GetBytes(secretKey));
        byte[] hashResult = hmac.ComputeHash(Encoding.UTF8.GetBytes(cipherText));
        return Base64UrlEncode(hashResult);
    }
}
