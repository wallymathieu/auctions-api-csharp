using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// An infrastructure command handler intended to be used together with a result type.
/// Note that the TryX( Raw raw, out T result ) is not possible to use due to <see cref="System.Linq.Expressions.Expression"/>
/// instead we we use a result type as seen in F#, Rust et.c.
/// </summary>
class FuncResultMutateCommandHandler<TEntity, TCommand, TResponse>(
    Func<TEntity, TCommand, IServiceProvider, TResponse> func,
    IServiceProvider serviceProvider) : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> where TEntity : IEntity
    where TResponse : IResult
{
    private readonly IRepository<TEntity> _repository=serviceProvider.GetRequiredService<IRepository<TEntity>>();
    private readonly AuctionDbContext _db= serviceProvider.GetRequiredService<AuctionDbContext>();

    public async ValueTask<TResponse?> Handle(TCommand cmd, CancellationToken cancellationToken = default)
    {
        var keyValueFactory = serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
        var entity = await _repository.FindAsync(keyValueFactory.Key(cmd), cancellationToken);

        if (entity is null)
        {
            return default;
        }
        var r = func(entity, cmd, serviceProvider);
        if (r.IsOk)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        return r;
    }
}