using App.Data;
using Auctions.Domain;
using Auctions.Models;
using Auctions.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly ILogger<AuctionsController> _logger;
    private readonly AuctionDbContext _dbContext;
    private readonly Mapper _mapper;
    private readonly CreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly CreateBidCommandHandler _createBidCommandHandler;

    public AuctionsController(ILogger<AuctionsController> logger, AuctionDbContext dbContext,
        Mapper mapper, CreateAuctionCommandHandler createAuctionCommandHandler, CreateBidCommandHandler createBidCommandHandler)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get()
    {
        return (await _dbContext.Auctions.Include(a=>a.Bids).ToListAsync())
            .Select(_mapper.MapAuctionToModel);
    }

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId)
    {
        var auction = await _dbContext.GetAuction(auctionId);
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
            case CreateBidCommandHandler.Result.Ok: return Ok();
            case CreateBidCommandHandler.Result.Error: return BadRequest(error);
            default:
            case CreateBidCommandHandler.Result.NotFound: return NotFound();
        }
    }
}