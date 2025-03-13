namespace Wallymathieu.Auctions.DomainModels.Bids;

/// <summary>
///     The bid user mapper gives a string representation of a user id. In the case when you have a private auction that
///     user id is the number that the user
/// </summary>
public interface IBidUserMapper
{
    string? GetUserString(UserId? userId);
}