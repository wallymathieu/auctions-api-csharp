namespace Auctions.Domain;

public interface ITime
{
    DateTimeOffset Now { get; }
}