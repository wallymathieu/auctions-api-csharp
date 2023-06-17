namespace Wallymathieu.Auctions;

/// <summary>
/// Note that this repository class is needed by the infrastructure logic
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity>
    where TEntity:IEntity
{
    ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken);
    Task<TEntity?> FindAsync(object identifier, CancellationToken cancellationToken);
}