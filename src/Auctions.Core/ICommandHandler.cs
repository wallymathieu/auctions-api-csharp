namespace Wallymathieu.Auctions;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken);
}
public interface IRepository<T>
    where T:IEntity
{
    ValueTask AddAsync(T entity, CancellationToken cancellationToken);
    Task<T?> FindAsync(object identifier, CancellationToken cancellationToken);
}