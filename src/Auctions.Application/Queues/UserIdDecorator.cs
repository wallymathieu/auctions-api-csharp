namespace Wallymathieu.Auctions.Application.Queues;
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