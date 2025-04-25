using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Models.V2;

namespace Wallymathieu.Auctions.Api.Controllers.V2;

[ApiController]
[Route("auctions"), ApiVersion("2.0")]
public class GetAuctionsController(
    AuctionMapper auctionMapper,
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
    public async Task<IEnumerable<AuctionModel>> GetV2(CancellationToken cancellationToken) =>
        from auction in await auctionQuery.GetAuctionsAsync(cancellationToken)
        select auctionMapper.MapAuctionToModel(auction);

    /// <summary>
    /// Get a single auction
    /// </summary>
    [HttpGet("{auctionId}", Name = "get_auction_v2")]
    public async Task<ActionResult<AuctionModel>> GetSingleV2(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await auctionQuery.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : auctionMapper.MapAuctionToModel(auction);
    }
}