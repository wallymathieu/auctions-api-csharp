using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Models.V1;

namespace Wallymathieu.Auctions.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
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
    [HttpGet("/auctions",Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await auctionQuery.GetAuctionsAsync(cancellationToken)
        select auctionMapper.MapAuctionToModel(auction);

    /// <summary>
    /// Get a single auction
    /// </summary>
    [HttpGet("/auction/{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await auctionQuery.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : auctionMapper.MapAuctionToModel(auction);
    }
}