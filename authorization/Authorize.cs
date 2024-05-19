using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace coinbase_bot.authorization;

public class Authorize : IAuthorize
{
    private string keyName = "";
    private readonly string keySecret = "";

    public string GenerateJwtToken(string sub, string name, string email, string role)
    {
        // Define the signing key and algorithm
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkey"));
        var signingAlgorithm = SignatureAlgorithm.HmacSha256;

        // Create the JWT
        var jwt_payload = new
        {
            sub = keyName,
            iss = "coinbase-cloud",
            nbf = DateTime.Now,
            exp = role
        };
        var jwt = new JwtSecurityToken(jwt_payload, signingKey, signingAlgorithm);

        // Convert the JWT to a string
        var jwtString = jwt.WriteToJson();

        return jwtString;
    }
}
