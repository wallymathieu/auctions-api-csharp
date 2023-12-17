namespace Wallymathieu.Auctions.Application.Queues;
public class UserIdDecorator<T>(T value, UserId? userId)
{
    public T Value { get; } = value;
    public UserId? UserId { get; } = userId;
}