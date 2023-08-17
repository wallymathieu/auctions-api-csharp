using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.Infrastructure.Queues;
using Wallymathieu.Auctions.Infrastructure.Services;

namespace Wallymathieu.Auctions.Tests.Helpers;

public class AsyncApiFixture<TAuth> : ApiFixture<TAuth> where TAuth : IApiAuth
{
    /// <summary>
    /// Note:
    /// <br/>- In order to be able to test the API with a message queue, we assume that it is somewhat equivalent to a API invocation
    /// directly with hidden side effects. Since the tests don't try to measure the time to only enqueue, we instead execute immediately.
    /// <br/>- An otherwise hidden side effect would be if the execution of the command throws an exception. We will get
    /// an exception directly in the enqueue call. Since the business logic does not use exceptions for business purposes
    /// this should be a good enough tradeoff.
    /// </summary>
    class FakeAsyncMessageQueue : IMessageQueue
    {
        private readonly ICreateAuctionCommandHandler _createAuctionCommandHandler;
        private readonly ICreateBidCommandHandler _createBidCommandHandler;
        private readonly ILogger<FakeAsyncMessageQueue> _logger;

        public FakeAsyncMessageQueue(ICreateAuctionCommandHandler createAuctionCommandHandler,
            ICreateBidCommandHandler createBidCommandHandler,
            ILogger<FakeAsyncMessageQueue> logger)
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

    public AsyncApiFixture(IDatabaseContextSetup dbContextSetup, TAuth auth) : base(dbContextSetup, auth) { }

    protected override void ConfigureServices(IServiceCollection services)
    {
        services.Remove(services.First(s => s.ServiceType == typeof(IMessageQueue)));
        services.AddScoped<IMessageQueue, FakeAsyncMessageQueue>();
    }
}