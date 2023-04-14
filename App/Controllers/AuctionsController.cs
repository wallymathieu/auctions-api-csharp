using Microsoft.AspNetCore.Mvc;

namespace App.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly ILogger<AuctionsController> _logger;

    public AuctionsController(ILogger<AuctionsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "auctions")]
    public IEnumerable<AuctionModel> Get()
    {
        return null;
    }
    [HttpPost(Name = "auctions")]
    public AuctionModel Post(CreateAuctionModel model)
    {
        return null;
    }

    public class CreateAuctionModel
    {
    }

    public class AuctionModel
    {
    }
}
