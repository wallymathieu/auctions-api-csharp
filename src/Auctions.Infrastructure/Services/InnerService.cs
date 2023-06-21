namespace Wallymathieu.Auctions.Infrastructure.Services;

internal record InnerService<T>(T Value)
{
    public T Service => Value;
}