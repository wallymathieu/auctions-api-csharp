using App.Data;
using Auctions.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly ILogger<AuctionsController> _logger;
    private readonly AuctionDbContext _dbContext;

    public AuctionsController(ILogger<AuctionsController> logger, AuctionDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get()
    {
        return (await _dbContext.Auctions.ToListAsync()).Select(MapAuctionToModel);
    }

    private AuctionModel MapAuctionToModel(TimedAscendingAuction arg) =>
        new(arg.Id.Id, arg.StartsAt, arg.Title, arg.Expiry, arg.User.ToString(), arg.Currency);

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<AuctionModel> GetSingle(long auctionId)
    {
        var auction = await _dbContext.Auctions.FindAsync(auctionId);
        return MapAuctionToModel(auction);
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
                MinRaise = new Amount(0, CurrencyCode.VAC),
                ReservePrice = new Amount(0, CurrencyCode.VAC),
                TimeFrame = TimeSpan.Zero,
            }
        };
        _dbContext.Auctions.Add(auction);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },MapAuctionToModel(auction));
    }

    public class CreateAuctionModel
    {
        public CurrencyCode Currency { get; set; }
        public DateTimeOffset StartsAt { get; set; }
        public DateTimeOffset EndsAt { get; set; }
        public string Title { get; set; }
    }

    public record AuctionModel(long Id, DateTimeOffset StartsAt,string Title,DateTimeOffset Expiry, string User,CurrencyCode Currency);
}
