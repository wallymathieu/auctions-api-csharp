namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Reference interface in order to be able to name the handler in the controller.
/// </summary>
public interface ICreateAuctionCommandHandler
{
    Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken = default);
}