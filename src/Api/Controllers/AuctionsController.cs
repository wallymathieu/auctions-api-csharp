using Microsoft.AspNetCore.Mvc;
using Wallymathieu.Auctions.ApiModels;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Data;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Services;

namespace Wallymathieu.Auctions.Api.Controllers;

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
    public async Task<IEnumerable<AuctionModel>> Get(CancellationToken cancellationToken) =>
        from auction in await _auctionRepository.GetAuctionsAsync(cancellationToken)
        select _mapper.MapAuctionToModel(auction);

    [HttpGet("{auctionId}", Name = "get_auction")]
    public async Task<ActionResult<AuctionModel>> GetSingle(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await _auctionRepository.GetAuctionAsync(auctionId, cancellationToken);
        return auction is null ? NotFound() : _mapper.MapAuctionToModel(auction);
    }

    [HttpPost(Name = "create_auction")]
    public async Task<ActionResult> Post(
        CreateAuctionCommand model, CancellationToken cancellationToken)
    {
        if (User?.Identity?.Name==null) // TODO use Authorize
        {
            return Unauthorized();
        }

        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.AuctionCommandQueueName, model, cancellationToken);
            return Accepted();
        }
        else
        {
            var auction = _mapper.MapAuctionToModel(await _createAuctionCommandHandler.Handle(model,cancellationToken));
            return CreatedAtAction(nameof(GetSingle),new {auctionId = auction.Id },auction);
        }
    }

    [HttpPost("{auctionId}/bids",Name = "add_bid")]
    public async Task<ActionResult> PostBid(long auctionId,
        CreateBidModel model, CancellationToken cancellationToken)
    {
        if (User?.Identity?.Name == null) // TODO use Authorize
        {
            return Unauthorized();
        }

        var cmd =  new CreateBidCommand(model.Amount, auctionId);
        if (_messageQueue.Enabled)
        {
            await _messageQueue.SendMessageAsync(QueuesModule.BidCommandQueueName, cmd, cancellationToken);
            return Accepted();
        }
        else
        {
            var result=
                await _createBidCommandHandler.Handle(cmd, cancellationToken);
            return result switch
            {
                Ok<Bid, Errors> => Ok(),
                Error<Bid, Errors> err => err.Value != Errors.UnknownAuction ? NotFound() : BadRequest(err.Value),
                _ => NotFound()
            };
        }
    }
}