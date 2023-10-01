using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Controllers;

[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionMapper _auctionMapper;
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly IAuctionRepository _auctionRepository;
    private readonly IUserContext _userContext;
    private readonly IMessageQueue _messageQueue;

    public AuctionsController(AuctionMapper auctionMapper,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        ICreateBidCommandHandler createBidCommandHandler,
        IAuctionRepository auctionRepository, IUserContext userContext, IMessageQueue messageQueue)
    {
        _auctionMapper = auctionMapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
        _auctionRepository = auctionRepository;
        _userContext = userContext;
        _messageQueue = messageQueue;
    }
    /// <summary>
    /// Get all auctions
    /// </summary>
    /// <remarks>
    /// Get a list of auctions.
    /// </remarks>
    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await _auctionRepository.GetAuctionsAsync(cancellationToken)
        select _auctionMapper.MapAuctionToModel(auction);
    /// <summary>
    /// Get a single auction
    /// </summary>
    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : _auctionMapper.MapAuctionToModel(auction);
    }
    /// <summary>
    /// Create an auction
    /// </summary>
    /// <remarks>
    /// Create an auction. Note that the auction models are restricted to be a closed bid auction either as a
    /// [First price sealed bid auction](https://en.wikipedia.org/wiki/First-price_sealed-bid_auction) or a
    /// [Vickrey auction](https://en.wikipedia.org/wiki/Vickrey_auction). It can also be a
    /// [Timed ascending auction also known as an English auction](https://en.wikipedia.org/wiki/English_auction).
    /// </remarks>
    [HttpPost(Name = "create_auction") , Authorize,
     ProducesResponseType(typeof(void),StatusCodes.Status200OK)]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        var auction = await _createAuctionCommandHandler.Handle(model, cancellationToken);
        var auctionModel =
            _auctionMapper.MapAuctionToModel(auction);
        if (_messageQueue.Enabled) // NOTE: we could have used a decorator here as well
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionResultQueueName,
                new UserIdDecorator<Auction>(auction, _userContext.UserId), cancellationToken);
        }
        return CreatedAtAction(nameof(GetSingle), new { auctionId = auctionModel.Id }, auctionModel);
    }
    /// <summary>
    /// Add a bid on an auction
    /// </summary>
    [HttpPost("{auctionId}/bids",Name = "add_bid"), Authorize,
     ProducesResponseType(typeof(void),StatusCodes.Status200OK),
     ProducesResponseType(typeof(Errors),StatusCodes.Status400BadRequest),
     ProducesResponseType(typeof(void),StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        var id = new AuctionId(auctionId);
        var cmd =  new CreateBidCommand(model.Amount, id);
        var result = await _createBidCommandHandler.Handle(cmd, cancellationToken);
        if (_messageQueue.Enabled) // NOTE: we could have used a decorator here as well
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidResultQueueName,
                new UserIdDecorator<Result<Bid,Errors>?>(result, _userContext.UserId), cancellationToken);
        }
        if (result is null) return NotFound();
        return result.Match<ActionResult>(ok => Ok(), err => err == Errors.UnknownAuction
            ? NotFound()
            : BadRequest(err));
    }
}