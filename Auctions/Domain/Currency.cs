namespace Auctions.Domain;

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
}