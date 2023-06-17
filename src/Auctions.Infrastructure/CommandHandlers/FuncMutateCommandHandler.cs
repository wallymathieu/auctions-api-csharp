using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

class FuncMutateCommandHandler<T, TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand<Unit> where T : IEntity
{
    private readonly Action<T, TCommand, IServiceProvider> _func;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<T> _repository;
    private readonly IAuctionDbContext _db;

    public FuncMutateCommandHandler(Action<T, TCommand, IServiceProvider> func, IServiceProvider serviceProvider)
    {
        _func = func;
        _serviceProvider = serviceProvider;
        _repository = _serviceProvider.GetRequiredService<IRepository<T>>();
        _db = serviceProvider.GetRequiredService<IAuctionDbContext>();
    }

    public async Task<Unit> Handle(TCommand cmd, CancellationToken cancellationToken)
    {
        var keyValueFactory = _serviceProvider.GetRequiredService<IKeyValueFactory<TCommand>>();
        var entity = await _repository.FindAsync(keyValueFactory.Key(cmd), cancellationToken);

        _func(entity, cmd, _serviceProvider);
        await _db.SaveChangesAsync();
        return Unit.Value;
    }
}

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