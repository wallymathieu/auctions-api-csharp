using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Api.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionMapper _auctionMapper;
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly IAuctionRepository _auctionRepository;

    public AuctionsController(AuctionMapper auctionMapper,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        ICreateBidCommandHandler createBidCommandHandler,
        IAuctionRepository auctionRepository)
    {
        _auctionMapper = auctionMapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
        _auctionRepository = auctionRepository;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await _auctionRepository.GetAuctionsAsync(cancellationToken)
        select _auctionMapper.MapAuctionToModel(auction);

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : _auctionMapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction") , Authorize]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var auction = await _createAuctionCommandHandler.Handle(model, cancellationToken);
        var auctionModel =
            _auctionMapper.MapAuctionToModel(auction);
        return CreatedAtAction(nameof(GetSingle), new { auctionId = auctionModel.Id }, auctionModel);
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid"), Authorize]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        var id = new AuctionId(auctionId);
        var cmd =  new CreateBidCommand(model.Amount, id);
        var result = await _createBidCommandHandler.Handle(cmd, cancellationToken);

        if (result is null) return NotFound();
        return result.Match<ActionResult>(ok => Ok(), err => err == Errors.UnknownAuction
            ? NotFound()
            : BadRequest(err));
    }
}