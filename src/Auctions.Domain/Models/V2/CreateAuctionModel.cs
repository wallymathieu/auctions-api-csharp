using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Models.V2;
/// <summary>
/// This model is used to create an auction.
/// When creating a timed ascending auction we need to specify the <see cref="MinRaise"/>, <see cref="ReservePrice"/> and <see cref="TimeFrame"/>.
/// When creating a single sealed bid auction we need to specify the <see cref="SingleSealedBidOptions"/> with either Vickrey or Blind.
/// </summary>
/// <param name="Title"></param>
/// <param name="Currency"></param>
/// <param name="StartsAt"></param>
/// <param name="EndsAt"></param>
/// <param name="MinRaise"></param>
/// <param name="ReservePrice"></param>
/// <param name="TimeFrame"></param>
/// <param name="SingleSealedBidOptions"></param>
public record CreateAuctionModel(
    [Required] string Title,
    CurrencyCode Currency,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    long? MinRaise,
    long? ReservePrice,
    TimeSpan? TimeFrame,
    SingleSealedBidOptions? SingleSealedBidOptions)
{
    public CreateAuctionCommand ToCommand()
    {
        return new CreateAuctionCommand(
            Title: Title,
            StartsAt: StartsAt,
            EndsAt: EndsAt,
            Currency: Currency,
            MinRaise: MinRaise,
            ReservePrice: ReservePrice,
            TimeFrame: TimeFrame,
            SingleSealedBidOptions: SingleSealedBidOptions);
    }
}