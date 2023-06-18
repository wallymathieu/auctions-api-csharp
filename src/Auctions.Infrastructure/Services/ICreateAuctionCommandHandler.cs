using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Infrastructure.Services;

public interface ICreateAuctionCommandHandler
{
    Task<Auction> Handle(CreateAuctionCommand model, CancellationToken cancellationToken);
}