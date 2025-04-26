using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;
class FuncCreateCommandHandler<TEntity, TCommand>(
    Func<TCommand, IServiceProvider, TEntity> func,
    IServiceProvider serviceProvider):
    ICommandHandler<TCommand, TEntity>
        where TCommand : ICommand<TEntity> where TEntity : IEntity
{
    private readonly IRepository<TEntity> _repository= serviceProvider.GetRequiredService<IRepository<TEntity>>();
    private readonly AuctionDbContext _db=serviceProvider.GetRequiredService<AuctionDbContext>();

    public async ValueTask<TEntity> Handle(TCommand cmd, CancellationToken cancellationToken = default)
    {
        var entity = func(cmd, serviceProvider);
        await _repository.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }
}