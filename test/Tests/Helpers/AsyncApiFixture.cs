using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class AsyncApiFixture<TAuth> : ApiFixture<TAuth> where TAuth : IApiAuth, new()
{
    class AsyncMessageQueue : IMessageQueue
    {
        private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
        private readonly ICreateBidCommandHandler _createBidCommandHandler;
        private readonly ILogger<AsyncMessageQueue> _logger;

        public AsyncMessageQueue(ICreateAuctionCommandHandler createAuctionCommandHandler,
            ICreateBidCommandHandler createBidCommandHandler,
            ILogger<AsyncMessageQueue> logger)
        {
            _createAuctionCommandHandler = createAuctionCommandHandler;
            _createBidCommandHandler = createBidCommandHandler;
            _logger = logger;
        }

        public bool Enabled => true;
        public async Task SendMessageAsync(string queueName, object command, CancellationToken cancellationToken)
        {
            switch (queueName)
            {
                case QueuesModule.AuctionCommandQueueName:
                {
                    var cmd = command as UserIdDecorator<CreateAuctionCommand>;
                    try
                    {
                        await _createAuctionCommandHandler.Handle(cmd!.Command, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,"Failed to add auction");
                        throw;
                    }
                    return;
                }
                case QueuesModule.BidCommandQueueName:
                {
                    var cmd = command as UserIdDecorator<CreateBidCommand>;
                    try
                    {
                        await _createBidCommandHandler.Handle(cmd!.Command, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,"Failed to add bid");
                        throw;
                    }
                    return;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public AsyncApiFixture(string db) : base(db) { }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.Remove(services.First(s => s.ServiceType == typeof(IMessageQueue)));
        services.AddScoped<IMessageQueue, AsyncMessageQueue>();
    }
}