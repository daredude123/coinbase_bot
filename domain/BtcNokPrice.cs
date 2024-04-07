namespace coinbase_bot.domain;

public class BtcNokPrice
{
    public float _amount { get; set; }
    public string _currencyBase { get; set; }
    public string _currency { get; set; }

    public BtcNokPrice(float amount, string currencyBase, string currency)
    {
        _amount = amount;
        _currencyBase = currencyBase;
        _currency = currency;
    }
}
