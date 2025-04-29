using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models.V1;
using Wallymathieu.Auctions.Application.Services;

namespace Wallymathieu.Auctions.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("auctions")]
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
    [HttpPost(Name = "create_auction"), Authorize,
     ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<ActionResult> Post(
        CreateAuctionModel model, CancellationToken cancellationToken)
    {
        if (model is null || !model.TryToConvert(out var command))
            return BadRequest("Invalid auction model");
        var auction = await createAuctionCommandHandler.Handle(command!, cancellationToken);
        var auctionModel =
            auctionMapper.MapAuctionToModel(auction);
        return CreatedAtAction("GetSingle", "GetAuctions",
            new { auctionId = auctionModel.Id }, auctionModel);
    }
}
