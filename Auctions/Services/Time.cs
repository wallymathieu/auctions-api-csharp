namespace Auctions.Services;

public class Time:ITime
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}

public interface ITime
{
    DateTimeOffset Now { get; }
}