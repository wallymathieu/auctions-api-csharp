using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

class FuncCreateCommandHandler<TEntity, TCommand> : ICommandHandler<TCommand, TEntity>
    where TCommand : ICommand<TEntity> where TEntity : IEntity
{
    private readonly Func<TCommand, IServiceProvider, TEntity> _func;
    private readonly IServiceProvider _serviceProvider;
    private readonly IRepository<TEntity> _repository;
    private readonly IAuctionDbContext _db;

    public FuncCreateCommandHandler(Func<TCommand, IServiceProvider, TEntity> func,
        IServiceProvider serviceProvider)
    {
        _func = func;
        _serviceProvider = serviceProvider;
        _repository = _serviceProvider.GetRequiredService<IRepository<TEntity>>();
        _db = serviceProvider.GetRequiredService<IAuctionDbContext>();
    }

    public async Task<TEntity> Handle(TCommand cmd, CancellationToken cancellationToken)
    {
        var entity = _func(cmd, _serviceProvider);
        await _repository.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync();
        return entity;
    }
}