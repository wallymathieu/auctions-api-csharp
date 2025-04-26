namespace Wallymathieu.Auctions.Application.Services;

/// <summary>
/// Reference interface in order to be able to name the handler in the controller.
/// </summary>
public interface ICreateBidCommandHandler
{
    Task<Result<Bid, Errors>?> Handle(CreateBidCommand model, CancellationToken cancellationToken = default);
}