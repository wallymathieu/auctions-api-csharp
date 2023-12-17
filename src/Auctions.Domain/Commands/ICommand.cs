
using MediatR;

namespace Wallymathieu.Auctions.Commands;

public interface ICommand<out TResponse>: IRequest<TResponse>
{
}

