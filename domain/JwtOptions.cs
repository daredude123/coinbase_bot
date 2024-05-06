namespace coinbase_bot.domain
{
    public record class JwtOptions(
        string Issuer,
        string Audience,
        string SigningKey,
        int ExpirationSeconds
    );
}
