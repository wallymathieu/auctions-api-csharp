using Auctions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Auctions.Data;

public class AuctionDbContext: DbContext
{
    public AuctionDbContext()
    {
    }
    public DbSet<TimedAscendingAuction> Auctions { get; set; }
    private PropertyBuilder<T> WithAuctionIdConversion<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<AuctionId, long>(v => v.Id, v => new AuctionId(v)));
    private PropertyBuilder<T> WithUserId<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<UserId, string>(v => v.Id, v => new UserId(v))).HasMaxLength(2000);
    private static PropertyBuilder<CurrencyCode> HasCurrencyCodeConversion(PropertyBuilder<CurrencyCode> propertyBuilder) =>
        propertyBuilder.HasConversion(new EnumToStringConverter<CurrencyCode>()).HasMaxLength(3);
    public async Task<TimedAscendingAuction?> GetAuction(long auctionId)
    {
        var auction = await Auctions.FindAsync(auctionId);
        if (auction is not null) await Entry(auction).Collection(p => p.Bids).LoadAsync();
        return auction;
    }
    public AuctionDbContext(DbContextOptions options):base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TimedAscendingAuction>(entity =>
        {
            entity.ToTable("Auctions");
            entity.HasKey(e => e.AuctionId);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(o => o.AuctionId).UseIdentityColumn();
            WithUserId(entity.Property(o => o.User));
            entity.OwnsOne<TimedAscendingOptions>(e=>e.Options);
            HasCurrencyCodeConversion(entity.Property(e => e.Currency));
            entity.HasMany(e => e.Bids).WithOne()
                .HasPrincipalKey(a=>a.AuctionId)
                .HasForeignKey("AuctionId");
        });
        
        builder.Entity<BidEntity>(entity =>
        {
            entity.ToTable("Bids");
            WithUserId(entity.Property(o => o.User));
            var amount = entity.OwnsOne(e => e.Amount);
            HasCurrencyCodeConversion(amount.Property(e => e.Currency));
        });
        
       
        base.OnModelCreating(builder);
    }

}