namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public interface IKeyValueFactory<T>
{
    object? Key(T obj);
}