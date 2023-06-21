namespace Wallymathieu.Auctions.Infrastructure.Queues;
public class UserIdDecorator<T>
{
    public UserIdDecorator(T command, UserId? userId)
    {
        Command = command;
        UserId = userId;
    }

    public T Command { get; }
    public UserId? UserId { get; }
}