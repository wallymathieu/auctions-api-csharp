using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Services;

namespace Auctions.Api.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly Mapper _mapper;
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly IAuctionRepository _auctionRepository;

    public AuctionsController(Mapper mapper, ICreateAuctionCommandHandler createAuctionCommandHandler, ICreateBidCommandHandler createBidCommandHandler, IAuctionRepository auctionRepository)
    {
        _mapper = mapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
        _auctionRepository = auctionRepository;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get()
    {
        return from auction in await _auctionRepository.GetAuctionsAsync()
            select _mapper.MapAuctionToModel(auction);
    }

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId)
    {
        var auction = await _auctionRepository.GetAuctionAsync(auctionId);
        return auction is null ? NotFound() : _mapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction")]
    public async Task<ActionResult<AuctionModel>> Post(
        CreateAuctionModel model)
    {
        if (this.User?.Identity?.Name==null)
        {
            return Unauthorized();
        }

        var auction = await _createAuctionCommandHandler.Handle(new UserId(this.User.Identity.Name), model);
        return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },auction);
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid")]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model)
    {
        if (User?.Identity?.Name == null)
        {
            return Unauthorized();
        }

        var (result, error) =
            await _createBidCommandHandler.Handle(auctionId, new UserId(User.Identity.Name), model);
        switch (result)
        {
            case CreateBidCommandResult.Ok: return Ok();
            case CreateBidCommandResult.Error: return BadRequest(error);
            default:
            case CreateBidCommandResult.NotFound: return NotFound();
        }
    }
}