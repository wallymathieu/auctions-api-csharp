using System.Globalization;

namespace Wallymathieu.Auctions.Infrastructure;

/// <summary>
///     Class intended to format time in a known format.
/// </summary>
public static class TimeFormatter
{
    private const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";

    public static string Format(DateTimeOffset dateTime)
    {
        return dateTime.ToString(TimeFormat, CultureInfo.InvariantCulture);
    }
}