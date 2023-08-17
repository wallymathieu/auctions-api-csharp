namespace Wallymathieu.Auctions.Infrastructure;

/// <summary>
/// Class intended to format time in a known format.
/// </summary>
public static class TimeFormatter
{
    public static string Format(DateTimeOffset dateTime) => dateTime.ToString(TimeFormat);
    private const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
}