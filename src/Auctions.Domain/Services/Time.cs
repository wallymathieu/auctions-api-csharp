namespace Wallymathieu.Auctions.Services;
/// <summary>
/// Note that we want to be able to fake time in tests.
/// </summary>
public class Time:ITime
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}

public interface ITime
{
    DateTimeOffset Now { get; }
}