using App.Data;
using App.Models;
using Auctions.Domain;
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
    private readonly ITime _time;

    public AuctionsController(ILogger<AuctionsController> logger, AuctionDbContext dbContext, ITime time)
    {
        _logger = logger;
        _dbContext = dbContext;
        _time = time;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get()
    {
        return (await _dbContext.Auctions.Include(a=>a.Bids).ToListAsync())
            .Select(MapAuctionToModel);
    }

    private AuctionModel MapAuctionToModel(TimedAscendingAuction arg) =>
        new(arg.Id.Id, arg.StartsAt, arg.Title, arg.Expiry, arg.User.ToString(), arg.Currency, 
            arg.GetBids(_time.Now)?.Select(MapBidToModel).ToArray()??Array.Empty<BidModel>());

    private static BidModel MapBidToModel(Bid arg)
    {
        return new BidModel
        {
            Amount = arg.Amount.ToString(),
            Bidder = arg.User.ToString()
        };
    }

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId)
    {
        var auction = await _dbContext.Auctions.Include(a=>a.Bids)
            .FirstOrDefaultAsync(a=>a.AuctionId == auctionId);
        return auction is null ? NotFound() : MapAuctionToModel(auction);
    }
    [HttpPost(Name = "create_auction")]
    public async Task<ActionResult<AuctionModel>> Post(
        CreateAuctionModel model)
    {
        if (this.User?.Identity?.Name==null)
        {
            return Unauthorized();
        }
        var auction = new TimedAscendingAuction
        {
            Currency = model.Currency,
            Expiry = model.EndsAt,
            StartsAt = model.StartsAt,
            Title = model.Title,
            User = new UserId(this.User.Identity.Name),
            Options =
            {
                MinRaise = 0, 
                ReservePrice = 0,
                TimeFrame = TimeSpan.Zero,
            }
        };
        _dbContext.Auctions.Add(auction);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },MapAuctionToModel(auction));
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid")]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model)
    {
        if (this.User?.Identity?.Name == null)
        {
            return Unauthorized();
        }

        var auction = await _dbContext.Auctions.FindAsync(auctionId);
        if (auction is null) return NotFound();
        if (!Amount.TryParse(model.Amount, out var amount))
        {
            return BadRequest();
        }
        if (auction.TryAddBid(_time.Now, 
                new Bid(new UserId(this.User.Identity.Name), amount!, _time.Now), out var error))
        {
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        else
        {
            return BadRequest(error);
        }
    }
}