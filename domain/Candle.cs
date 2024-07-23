namespace coinbase_bot.domain;

public class Candle
{
    public long Start { get; set; }
    public decimal Low { get; set; }
    public decimal High { get; set; }
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return $"Candle /n" +
        $"Start {Start}/n" +
        $"Low  {Low}/n" +
        $"High {High}/n" +
        $"Open {Open}/n" +
        $"Close {Close}/n" +
        $"Volume {Volume}/n";
    }
}
