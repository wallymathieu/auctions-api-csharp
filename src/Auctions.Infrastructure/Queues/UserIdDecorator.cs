namespace Wallymathieu.Auctions.Infrastructure.Queues;

public class UserIdDecorator<T>(T value, UserId? userId)
{
    public T Value { get; } = value;
    public UserId? UserId { get; } = userId;
}