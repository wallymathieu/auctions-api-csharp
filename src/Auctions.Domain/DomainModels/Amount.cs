using System.Globalization;
using System.Text.RegularExpressions;

namespace Wallymathieu.Auctions.DomainModels;

[Serializable]
public partial record Amount(long Value, CurrencyCode Currency): IComparable<Amount>
{
    public static Amount Zero(CurrencyCode c) => new(0L, c);

    public override string ToString()
    {
        return $"{Currency}{Value}";
    }

    public static bool TryParse(string amount, out Amount? value)
    {
        var match = AmountRegex().Match(amount);
        if (match.Success)
        {
            var currencyString = match.Groups["currency"].Value;
            var v = match.Groups["value"].Value;
            if (DomainModels.Currency.TryParse(currencyString, out var currency) && currency != null)
            {
                value = new Amount(long.Parse(v, CultureInfo.InvariantCulture), currency);
                return true;
            }
        }

        value = default;
        return false;
    }

    public static Amount operator +(Amount a1, Amount a2) => Add(a1, a2);

    public static Amount Add(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));

        AssertSameCurrency(a1, a2);

        return a1 with { Value = a1.Value + a2.Value };
    }

    public static Amount operator -(Amount a1, Amount a2) => Subtract(a1, a2);

    public static Amount Subtract(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));
        AssertSameCurrency(a1, a2);
        return a1 with { Value = a1.Value - a2.Value };
    }

    public static bool operator <=(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));
        AssertSameCurrency(a1, a2);
        return a1.Value<=a2.Value;
    }

    private static void AssertSameCurrency(Amount a1, Amount a2)
    {
        var currency = a1.Currency;
        var currency2 = a2.Currency;
        if (!currency.Equals(currency2))
        {
            throw new ArgumentException("not defined for two different currencies");
        }
    }

    public static bool operator >=(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));
        AssertSameCurrency(a1, a2);

        return a1.Value >= a2.Value;
    }
    public static bool operator >(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));
        AssertSameCurrency(a1, a2);

        return a1.Value > a2.Value;
    }

    public static bool operator <(Amount a1, Amount a2)
    {
        ArgumentNullException.ThrowIfNull(a1, nameof(a1));
        ArgumentNullException.ThrowIfNull(a2, nameof(a2));
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
            return amount!;
        }

        throw new ArgumentException("Could not parse value");
    }

    [GeneratedRegex("(?<currency>[A-Z]+)(?<value>[0-9]+)")]
    private static partial Regex AmountRegex();
}