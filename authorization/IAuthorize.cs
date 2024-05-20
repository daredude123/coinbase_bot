namespace coinbase_bot.authorization
{
    public interface IAuthorize
    {
        public string MakeJwt(Header header, Payload payload, string secretKey);
    }
}
