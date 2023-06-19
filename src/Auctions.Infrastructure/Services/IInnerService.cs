namespace Wallymathieu.Auctions.Infrastructure.Services;

internal interface IInnerService<out T>
{
    T Service { get; }
}