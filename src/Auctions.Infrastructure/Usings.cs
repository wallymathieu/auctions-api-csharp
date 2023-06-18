global using Wallymathieu.Auctions.Commands;
global using ICreateBidCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateBidCommand,
    Wallymathieu.Auctions.IResult<Wallymathieu.Auctions.DomainModels.Bid, Wallymathieu.Auctions.DomainModels.Errors>>;
global using ICreateAuctionCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateAuctionCommand,
    Wallymathieu.Auctions.DomainModels.Auction>;
global using Wallymathieu.Auctions.Commands;
global using Wallymathieu.Auctions.DomainModels;
global using Wallymathieu.Auctions.Data;
