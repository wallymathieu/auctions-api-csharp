using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models.V2;

namespace Wallymathieu.Auctions.Api.Controllers.V2;

[ApiController]
[Route("auctions"), ApiVersion("2.0")]
public class CreateAuctionController(
    AuctionMapper auctionMapper,
    ICreateAuctionCommandHandler createAuctionCommandHandler)
    : ControllerBase
{
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
        CreateAuctionModel model, CancellationToken cancellationToken)
    {
        var auction = await createAuctionCommandHandler.Handle(model.ToCommand(), cancellationToken);
        var auctionModel =
            auctionMapper.MapAuctionToModel(auction);
        return CreatedAtAction("GetSingle", "GetAuctions", new { auctionId = auctionModel.Id }, auctionModel);
    }
}