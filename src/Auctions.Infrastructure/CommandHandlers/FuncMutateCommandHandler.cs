using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// An infrastructure command handler. Used in order to be able to implement mutation command handlers.
/// Intention is to have a class that accepts a compiled lambda.
/// </summary>
class FuncMutateCommandHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> where TEntity : IEntity
{
    private readonly Func<TEntity, TCommand, IServiceProvider, TResponse> _func;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<TEntity> _repository;
    private readonly AuctionDbContext _db;

    public FuncMutateCommandHandler(Func<TEntity, TCommand, IServiceProvider, TResponse> func,
        IServiceProvider serviceProvider)
    {
        _func = func;
        _serviceProvider = serviceProvider;
        _repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        _db = serviceProvider.GetRequiredService<AuctionDbContext>();
    }

    public async Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken)
    {
        var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
        var entity = await _repository.FindAsync(keyValueFactory.Key(cmd), cancellationToken);
        if (entity is null)
        {
            return default;
        }

        var r = _func(entity, cmd, _serviceProvider);
        await _db.SaveChangesAsync(cancellationToken);

        return r;
    }
}