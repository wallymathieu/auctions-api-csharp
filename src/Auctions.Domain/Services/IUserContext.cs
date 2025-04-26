using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Services;

/// <summary>
/// UserId is part of the ambient context for a request. It is typically set by the authentication middleware.
/// </summary>
public interface IUserContext
{
    UserId? UserId { get; }
}