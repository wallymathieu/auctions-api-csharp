global using ICreateBidCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateBidCommand,
    Wallymathieu.Auctions.Result<Wallymathieu.Auctions.DomainModels.Bid, Wallymathieu.Auctions.DomainModels.Errors>>;
global using ICreateAuctionCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateAuctionCommand,
    Wallymathieu.Auctions.DomainModels.Auction>;
global using System.Text.Json;
global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Extensions.Logging;
global using Wallymathieu.Auctions.Commands;
global using Wallymathieu.Auctions.DomainModels;
global using Wallymathieu.Auctions.Infrastructure.Queues;
global using Wallymathieu.Auctions.Infrastructure.Services;
