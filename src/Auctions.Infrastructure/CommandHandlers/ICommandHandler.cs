namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand cmd, CancellationToken cancellationToken);
}