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
    private readonly IAuctionRepository _auctionRepository;
    private readonly IMessageQueue _messageQueue;
    private readonly IUserContext _userContext;

    public AuctionsController(AuctionMapper auctionMapper,
        IAuctionRepository auctionRepository,
        IMessageQueue messageQueue,
        IUserContext userContext)
    {
        _auctionMapper = auctionMapper;
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
        var auction = await _auctionRepository.GetAuctionAsync(new AuctionId(auctionId), cancellationToken);
        return auction is null ? NotFound() : _auctionMapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction") , Authorize]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        await _messageQueue.SendMessageAsync(QueuesModule.AuctionCommandQueueName,
            new UserIdDecorator<CreateAuctionCommand>(model,_userContext.UserId), cancellationToken);
        return Accepted();
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid"), Authorize]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        var cmd =  new CreateBidCommand(model.Amount, new AuctionId(auctionId));
        await _messageQueue.SendMessageAsync(QueuesModule.BidCommandQueueName,
            new UserIdDecorator<CreateBidCommand>(cmd,_userContext.UserId), cancellationToken);
        return Accepted();
    }
}