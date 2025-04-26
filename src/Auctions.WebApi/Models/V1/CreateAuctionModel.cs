using System.ComponentModel.DataAnnotations;
using Wallymathieu.Auctions.Commands;
using Wallymathieu.Auctions.DomainModels;
using Wallymathieu.Auctions.DomainModels.AuctionTypes;

namespace Wallymathieu.Auctions.Api.Models.V1;

/// <summary>
/// This model is used to create an auction.
/// </summary>
/// <param name="Title"></param>
/// <param name="StartsAt"></param>
/// <param name="EndsAt"></param>
/// <param name="Currency"></param>
/// <param name="Type">Is either nothing or "English|{reservePrice}|{minRaise}|{timeFrame.Ticks}", "Blind" or "Vickrey"</param>
/// <param name="Open">If bidding should be done in the open</param>
public record CreateAuctionModel(
    [Required] string Title,
    [Required] DateTimeOffset StartsAt,
    [Required] DateTimeOffset EndsAt,
    CurrencyCode Currency,
    string? Type,
    bool Open)
{
    public bool TryToConvert(out CreateAuctionCommand? command)
    {
        TimedAscendingOptions? timedAscendingOptions = null;
        SingleSealedBidOptions? singleSealedBidOptions = null;
        if (!string.IsNullOrWhiteSpace(Type))
        {
            if (DomainModels.AuctionTypes.AuctionType.TryParse(Type, out var auctionTypeWithOptions))
            {
                switch (auctionTypeWithOptions)
                {
                    case EnglishAuctionType english:
                        timedAscendingOptions = new TimedAscendingOptions
                        {
                            MinRaise = english.MinRaise,
                            ReservePrice = english.ReservePrice,
                            TimeFrame = english.TimeFrame
                        };
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
            TimedAscendingOptions: timedAscendingOptions,
            SingleSealedBidOptions: singleSealedBidOptions,
            Open: Open);
        return true;
    }
}

