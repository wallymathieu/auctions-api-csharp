using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("auctions")]
public class CreateBidController(
    ICreateBidCommandHandler createBidCommandHandler)
    : ControllerBase
{
    /// <summary>
    /// Add a bid on an auction
    /// </summary>
    [HttpPost("{auctionId}/bids", Name = "add_bid"), Authorize,
     ProducesResponseType(typeof(void), StatusCodes.Status200OK),
     ProducesResponseType(typeof(Errors), StatusCodes.Status400BadRequest),
     ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        var result = await createBidCommandHandler.Handle(
            new CreateBidCommand(model.Amount, new AuctionId(auctionId)), cancellationToken);

        if (result is null) return NotFound();
        return result.Match<ActionResult>(_ => Ok(), err => err == Errors.UnknownAuction
            ? NotFound()
            : BadRequest(err));
    }
}