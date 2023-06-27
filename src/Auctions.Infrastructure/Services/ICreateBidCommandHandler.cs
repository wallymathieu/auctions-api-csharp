using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Infrastructure.Services;

public interface ICreateBidCommandHandler
{
    Task<Result<Bid, Errors>?> Handle(CreateBidCommand model, CancellationToken cancellationToken);
}