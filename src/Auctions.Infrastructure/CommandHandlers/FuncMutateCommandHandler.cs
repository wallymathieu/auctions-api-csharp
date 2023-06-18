using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Data;
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
    private readonly IAuctionDbContext _db;

    public FuncMutateCommandHandler(Func<TEntity, TCommand, IServiceProvider, TResponse> func,
        IServiceProvider serviceProvider)
    {
        _func = func;
        _serviceProvider = serviceProvider;
        _repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        _db = serviceProvider.GetRequiredService<IAuctionDbContext>();
    }

    public async Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken)
    {
        var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
        var entity = await _repository.FindAsync(keyValueFactory.Key(cmd), cancellationToken);

        var r = _func(entity, cmd, _serviceProvider);
        await _db.SaveChangesAsync();

        return r;
    }
}