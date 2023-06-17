namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public interface IKeyValueFactory<in TEntity>
{
    object? Key(TEntity obj);
}