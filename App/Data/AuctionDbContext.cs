using Auctions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.Data;

public class AuctionDbContext: DbContext
{
    private PropertyBuilder<T> WithAuctionIdConversion<T>(PropertyBuilder<T> self) => self.HasConversion(new ValueConverter<AuctionId, long>(v => v.Id, v => new AuctionId(v)));

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TimedAscendingAuction>(entity =>
        {
            WithAuctionIdConversion(entity.Property(o => o.Id));
        });
        builder.Entity<Bid>(entity =>
        {
            WithAuctionIdConversion(entity.Property(o => o.AuctionId));
        });
        base.OnModelCreating(builder);
    }
}