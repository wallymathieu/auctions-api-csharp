using MediatR;

namespace Wallymathieu.Auctions.Infrastructure.CommandHandlers;

public interface ICommandHandler<in TCommand, TResponse>
    :IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}