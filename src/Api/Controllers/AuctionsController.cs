using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.Domain;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Models;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Controllers;
using ICreateAuctionCommandHandler= ICommandHandler<CreateAuctionCommand, Auction>;
using ICreateBidCommandHandler= ICommandHandler<CreateBidCommand, IResult<Bid,Errors>>;
[ApiController]
[Route("auctions")]
public class AuctionsController : ControllerBase
{
    private readonly Mapper _mapper;
    private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
    private readonly ICreateBidCommandHandler _createBidCommandHandler;
    private readonly IAuctionRepository _auctionRepository;
    private readonly IMessageQueue _messageQueue;

    public AuctionsController(Mapper mapper,
        ICreateAuctionCommandHandler createAuctionCommandHandler,
        ICreateBidCommandHandler createBidCommandHandler,
        IAuctionRepository auctionRepository,
        IMessageQueue messageQueue)
    {
        _mapper = mapper;
        _createAuctionCommandHandler = createAuctionCommandHandler;
        _createBidCommandHandler = createBidCommandHandler;
        _auctionRepository = auctionRepository;
        _messageQueue = messageQueue;
    }

    [HttpGet(Name = "get_auctions")]
    public async Task<IEnumerable<AuctionModel>> Get() =>
        from auction in await _auctionRepository.GetAuctionsAsync()
        select _mapper.MapAuctionToModel(auction);

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId)
    {
        var auction = await _auctionRepository.GetAuctionAsync(auctionId);
        return auction is null ? NotFound() : _mapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction")]
    public async Task<ActionResult> Post(
        CreateAuctionModel model, CancellationToken token)
    {
        if (User?.Identity?.Name==null)
        {
            return Unauthorized();
        }

        var userId = new UserId(User.Identity.Name);
        var command = new CreateAuctionCommand(userId, model);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionCommandQueueName, command, token);
            return Accepted();
        }
        else
        {
            var auction = _mapper.MapAuctionToModel(await _createAuctionCommandHandler.Handle(command,token));
            return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },auction);
        }
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid")]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken token)
    {
        if (User?.Identity?.Name == null)
        {
            return Unauthorized();
        }

        var userId = new UserId(User.Identity.Name);
        var cmd = new CreateBidCommand(auctionId, userId, model);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidCommandQueueName, cmd, token);
            return Accepted();
        }
        else
        {
            var result=
                await _createBidCommandHandler.Handle(cmd, token);
            return result switch
            {
                Ok<Bid, Errors> => Ok(),
                Error<Bid, Errors> err => err.Value != Errors.UnknownAuction ? NotFound() : BadRequest(err.Value),
                _ => NotFound()
            };
        }
    }
}