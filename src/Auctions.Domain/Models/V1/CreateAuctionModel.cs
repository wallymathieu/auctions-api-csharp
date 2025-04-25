using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.DomainModels.AuctionTypes;

namespace Wallymathieu.Auctions.Models.V1;

/// <summary>
/// This model is used to create an auction.
/// </summary>
/// <param name="Title"></param>
/// <param name="StartsAt"></param>
/// <param name="EndsAt"></param>
/// <param name="Currency"></param>
/// <param name="Type">Is either nothing or "English|{reservePrice}|{minRaise}|{timeFrame.Ticks}", "Blind" or "Vickrey"</param>
public record CreateAuctionModel(
    [Required] string Title,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    CurrencyCode Currency,
    string? Type)
{
    public bool TryToConvert(out CreateAuctionCommand? command)
    {
        long? MinRaise = null;
        long? ReservePrice = null;
        TimeSpan? TimeFrame = null;
        SingleSealedBidOptions? singleSealedBidOptions = null;
        if (!string.IsNullOrWhiteSpace(Type))
        {
            if (DomainModels.AuctionTypes.AuctionType.TryParse(Type, out var auctionTypeWithOptions))
            {
                switch (auctionTypeWithOptions)
                {
                    case EnglishAuctionType english:
                        MinRaise = english.MinRaise;
                        ReservePrice = english.ReservePrice;
                        TimeFrame = english.TimeFrame;
                        break;
                    case BlindAuctionType:
                        singleSealedBidOptions = SingleSealedBidOptions.Blind;
                        break;
                    case VickreyAuctionType:
                        singleSealedBidOptions = SingleSealedBidOptions.Vickrey;
                        break;
                    default:
                        command = null;
                        return false;
                }
            }
            else
            {
                command = null;
                return false;
            }
        }

        command = new CreateAuctionCommand(
            Title: Title,
            StartsAt: StartsAt,
            EndsAt: EndsAt,
            Currency: Currency,
            MinRaise: MinRaise,
            ReservePrice: ReservePrice,
            TimeFrame: TimeFrame,
            SingleSealedBidOptions: singleSealedBidOptions);
        return true;
    }
}

