using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models.V1;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;

namespace Wallymathieu.Auctions.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("auctions")]
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
    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await auctionQuery.GetAuctionsAsync(cancellationToken)
        select auctionMapper.MapAuctionToModel(auction);

    /// <summary>
    /// Get a single auction
    /// </summary>
    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await auctionQuery.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : auctionMapper.MapAuctionToModel(auction);
    }
}
