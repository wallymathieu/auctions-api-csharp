namespace Wallymathieu.Auctions.Infrastructure.Services;

internal record InnerService<T>(T Value) : IInnerService<T>
{
    public T Service => Value;
}