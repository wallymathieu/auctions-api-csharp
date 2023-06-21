using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Services;

public interface IUserContext
{
    UserId? UserId { get; }
}