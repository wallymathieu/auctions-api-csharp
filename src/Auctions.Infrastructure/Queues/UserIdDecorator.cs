namespace Wallymathieu.Auctions.Infrastructure.Queues;
public class UserIdDecorator<T>
{
    public UserIdDecorator(T value, UserId? userId)
    {
        Value = value;
        UserId = userId;
    }

    public T Value { get; }
    public UserId? UserId { get; }
}