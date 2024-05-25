namespace coinbase_bot.authorization;

public class Header
{
    public string? Alg { get; set; }
    public string? Typ { get; set; }
    public string? Kid { get; set; }
    public string? Nonce { get; set; }
}
