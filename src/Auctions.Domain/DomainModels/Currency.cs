namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public record Currency(CurrencyCode Code)
{
    public override string ToString()
    {
        return Code.ToString();
    }

    public static bool TryParse(string c, out Currency? value)
    {
        if (Enum.TryParse(c, out CurrencyCode result))
        {
            value = new Currency(result);
            return true;
        }

        value = default;
        return false;
    }

    public static implicit operator CurrencyCode(Currency d)
    {
        ArgumentNullException.ThrowIfNull(d);
        return d.Code;
    }

    public static implicit operator Currency(CurrencyCode d) => new(d);

    public Currency ToCurrency() => new(Code);
    public CurrencyCode ToCurrencyCode() => Code;
}