using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Api.Models.V2;
/// <summary>
/// This model is used to create an auction.
/// When creating a timed ascending auction we need to specify the <see cref="TimedAscendingOptions"/>.
/// When creating a single sealed bid auction we need to specify the <see cref="SingleSealedBidOptions"/> with either Vickrey or Blind.
/// </summary>
/// <param name="Title"></param>
/// <param name="Currency"></param>
/// <param name="StartsAt"></param>
/// <param name="EndsAt"></param>
/// <param name="TimedAscendingOptions"></param>
/// <param name="SingleSealedBidOptions"></param>
/// <param name="OpenBidders">If bidding should be done in the open</param>
public record CreateAuctionModel(
    [Required] string Title,
    CurrencyCode Currency,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    TimedAscendingOptions? TimedAscendingOptions,
    SingleSealedBidOptions? SingleSealedBidOptions,
    bool OpenBidders)
{
    public CreateAuctionCommand ToCommand()
    {
        return new CreateAuctionCommand(
            Title: Title,
            StartsAt: StartsAt,
            EndsAt: EndsAt,
            Currency: Currency,
            TimedAscendingOptions: TimedAscendingOptions,
            SingleSealedBidOptions: SingleSealedBidOptions,
            Open: OpenBidders);
    }
}