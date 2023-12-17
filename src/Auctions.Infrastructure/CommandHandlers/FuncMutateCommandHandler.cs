using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// An infrastructure command handler. Used in order to be able to implement mutation command handlers.
/// Intention is to have a class that accepts a compiled lambda.
/// </summary>
class FuncMutateCommandHandler<TEntity, TCommand, TResponse>(
    Func<TEntity, TCommand, IServiceProvider, TResponse> func,
    IServiceProvider serviceProvider):
    ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse> where TEntity : IEntity
{
    private readonly IRepository<TEntity> _repository=serviceProvider.GetRequiredService<IRepository<TEntity>>();
    private readonly AuctionDbContext _db=serviceProvider.GetRequiredService<AuctionDbContext>();

    public async Task<TResponse?> Handle(TCommand cmd, CancellationToken cancellationToken = default)
    {
        var keyValueFactory = serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
        var entity = await _repository.FindAsync(keyValueFactory.Key(cmd), cancellationToken);
        if (entity is null)
        {
            return default;
        }

        var r = func(entity, cmd, serviceProvider);
        await _db.SaveChangesAsync(cancellationToken);

        return r;
    }
}