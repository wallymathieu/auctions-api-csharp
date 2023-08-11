using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Api.Middleware.Auth;
using Wallymathieu.Auctions.Api.Models;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Data;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;
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
    private readonly IMessageQueue _messageQueue;
    private readonly IUserContext _userContext;

    public AuctionsController(AuctionMapper auctionMapper,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        ICreateBidCommandHandler createBidCommandHandler,
        IAuctionRepository auctionRepository,
        IMessageQueue messageQueue,
        IUserContext userContext)
    {
        _auctionMapper = auctionMapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
        _auctionRepository = auctionRepository;
        _messageQueue = messageQueue;
        _userContext = userContext;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await _auctionRepository.GetAuctionsAsync(cancellationToken)
        select _auctionMapper.MapAuctionToModel(auction);

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionAsync(auctionId, cancellationToken);
        return auction is null ? NotFound() : _auctionMapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction") , Authorize]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionCommandQueueName, new UserIdDecorator<CreateAuctionCommand>(model,_userContext.UserId), cancellationToken);
            return Accepted();
        }
        else
        {
            var auction = _auctionMapper.MapAuctionToModel(await _createAuctionCommandHandler.Handle(model,cancellationToken));
            return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },auction);
        }
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid"), Authorize]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        var cmd =  new CreateBidCommand(model.Amount, auctionId);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidCommandQueueName, new UserIdDecorator<CreateBidCommand>(cmd,_userContext.UserId), cancellationToken);
            return Accepted();
        }
        else
        {
            var result = await _createBidCommandHandler.Handle(cmd, cancellationToken);
            if (result is null) return NotFound();
            return result.Match<ActionResult>(ok => Ok(), err => err == Errors.UnknownAuction
                ? NotFound()
                : BadRequest(err));
        }
    }
}