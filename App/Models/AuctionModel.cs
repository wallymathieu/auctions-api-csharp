using App.Controllers;
using Auctions.Domain;

namespace App.Models;

public record AuctionModel(long Id, DateTimeOffset StartsAt,string Title,DateTimeOffset Expiry, string User,CurrencyCode Currency, BidModel[] Bids);