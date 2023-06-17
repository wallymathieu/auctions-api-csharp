using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
/// <summary>
/// An infrastructure command handler intended to be used together with a result type.
/// Note that the TryX( Raw raw, out T result ) is not possible to use due to <see cref="System.Linq.Expressions.Expression"/>
/// instead we we use a result type as seen in F#, Rust et.c.
/// </summary>
class FuncResultMutateCommandHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse> where TEntity : IEntity
    where TResponse : IResult
{
    private readonly Func<TEntity, TCommand, IServiceProvider, TResponse> _func;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<TEntity> _repository;
    private readonly IAuctionDbContext _db;

    public FuncResultMutateCommandHandler(Func<TEntity, TCommand, IServiceProvider, TResponse> func,
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
        if (r.IsOk)
        {
            await _db.SaveChangesAsync();
        }
        return r;
    }
}