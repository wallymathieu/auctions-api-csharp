using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Services;
using Wallymathieu.Auctions.Models;

namespace Wallymathieu.Auctions.Api.Controllers.V2;

[ApiController]
[Route("auctions"), ApiVersion("2.0")]
public class AuctionsController(
    AuctionMapper auctionMapper,
    ICreateAuctionCommandHandler createAuctionCommandHandler,
    ICreateBidCommandHandler createBidCommandHandler,
    IAuctionQuery auctionQuery)
    : ControllerBase
{
    /// <summary>
    /// Get all auctions
    /// </summary>
    /// <remarks>
    /// Get a list of auctions.
    /// </remarks>
    [HttpGet(Name = "get_auctions_v2")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await auctionQuery.GetAuctionsAsync(cancellationToken)
        select auctionMapper.MapAuctionToModel(auction);

    /// <summary>
    /// Get a single auction
    /// </summary>
    [HttpGet("{auctionId}", Name = "get_auction_v2")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await auctionQuery.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : auctionMapper.MapAuctionToModel(auction);
    }

    /// <summary>
    /// Create an auction
    /// </summary>
    /// <remarks>
    /// Create an auction. Note that the auction models are restricted to be a closed bid auction either as a
    /// [First price sealed bid auction](https://en.wikipedia.org/wiki/First-price_sealed-bid_auction) or a
    /// [Vickrey auction](https://en.wikipedia.org/wiki/Vickrey_auction). It can also be a
    /// [Timed ascending auction also known as an English auction](https://en.wikipedia.org/wiki/English_auction).
    /// </remarks>
    [HttpPost(Name = "create_auction_v2"), Authorize,
     ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var auction = await createAuctionCommandHandler.Handle(model, cancellationToken);
        var auctionModel =
            auctionMapper.MapAuctionToModel(auction);
        return CreatedAtAction(nameof(GetSingle), new { auctionId = auctionModel.Id }, auctionModel);
    }

    /// <summary>
    /// Add a bid on an auction
    /// </summary>
    [HttpPost("{auctionId}/bids", Name = "add_bid_v2"), Authorize,
     ProducesResponseType(typeof(void), StatusCodes.Status200OK),
     ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest),
     ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var id = new AuctionId(auctionId);
        var cmd = new CreateBidCommand(model.Amount, id);
        var result = await createBidCommandHandler.Handle(cmd, cancellationToken);

        if (result is null) return NotFound();
        return result.Match<ActionResult>(_ => Ok(), err => err == Errors.UnknownAuction
            ? NotFound()
            : BadRequest(err));
    }
}