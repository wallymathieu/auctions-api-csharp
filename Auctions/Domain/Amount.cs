using System.Text.RegularExpressions;

namespace Auctions.Domain;

[Serializable]
public record Amount(long Value, Currency Currency): IComparable<Amount>
{
    public static Amount zero(Currency c)
    {
        return new Amount(0L, c);
    }


    public override string ToString()
    {
        return $"{Currency}{Value}";
    }

    public static bool TryParse(string amount, out Amount? value)
    {
        var match = new Regex("(?<currency>[A-Z]+)(?<value>[0-9]+)").Match(amount);
        if (match.Success)
        {
            var currencyString = match.Groups["currency"].Value;
            var v = match.Groups["value"].Value;
            if (Currency.TryParse(currencyString, out var currency) && currency != null)
            {
                value = new Amount(long.Parse(v), currency);
                return true;
            }
        }

        value = default;
        return false;
    }

    public static Amount operator +(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1,a2);

        return a1 with { Value = a1.Value + a2.Value };
    }

    public static Amount operator -(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1, a2);

        return a1 with { Value = a1.Value - a2.Value };
    }
    public static bool operator <=(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1, a2);
        return a1.Value<=a2.Value;
    }

    private static void AssertSameCurrency(Amount a1, Amount a2)
    {
        var currency = a1.Currency;
        var currency2 = a2.Currency;
        if (!currency.Equals(currency2))
        {
            throw new Exception("not defined for two different currencies");
        }
    }

    public static bool operator >=(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1, a2);

        return a1.Value >= a2.Value;
    }
    public static bool operator >(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1, a2);

        return a1.Value > a2.Value;
    }

    public static bool operator <(Amount a1, Amount a2)
    {
        AssertSameCurrency(a1, a2);

        return a1.Value < a2.Value;
    }

    public int CompareTo(Amount? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Value.CompareTo(other.Value);
    }

    public static Amount Parse(string s)
    {
        if (TryParse(s, out var amount))
        {
            return amount;
        }

        throw new ArgumentException();
    }
}