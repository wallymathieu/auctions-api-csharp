global using Wallymathieu.Auctions.Domain;
global using Wallymathieu.Auctions.Commands;
global using ICreateBidCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateBidCommand,
    Wallymathieu.Auctions.IResult<Wallymathieu.Auctions.Domain.Bid, Wallymathieu.Auctions.Domain.Errors>>;
global using ICreateAuctionCommandHandler= Wallymathieu.Auctions.Infrastructure.CommandHandlers.ICommandHandler<
    Wallymathieu.Auctions.Commands.CreateAuctionCommand,
    Wallymathieu.Auctions.Domain.Auction>;