using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wallymathieu.Auctions.DomainModels;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public class AuctionDbContext: DbContext
{
    public AuctionDbContext()
    {
    }

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options):base(options)
    {
    }
    public DbSet<Auction> Auctions { get; set; }
    private PropertyBuilder<T> WithAuctionIdConversion<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<AuctionId, long>(v => v.Id, v => new AuctionId(v)));
    private PropertyBuilder<T> WithUserId<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<UserId, string>(v => v.Id, v => new UserId(v))).HasMaxLength(2000);
    private static PropertyBuilder<CurrencyCode> HasCurrencyCodeConversion(PropertyBuilder<CurrencyCode> propertyBuilder) =>
        propertyBuilder.HasConversion(new EnumToStringConverter<CurrencyCode>()).HasMaxLength(3);

    public async ValueTask<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        return await Auctions.AsNoTracking().Include(a => a.Bids).ToListAsync(cancellationToken);
    }

    public async ValueTask<Auction?> GetAuction(long auctionId, CancellationToken cancellationToken)
    {
        var auction = await Auctions.FindAsync(auctionId, cancellationToken);
        if (auction is not null) await Entry(auction).Collection(p => p.Bids).LoadAsync(cancellationToken);
        return auction;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Auction>(entity =>
            {
                entity.HasDiscriminator(b => b.AuctionType).IsComplete(false);
                entity.ToTable("Auctions");
                entity.HasKey(e => e.AuctionId);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(o => o.AuctionId).UseIdentityColumn();
                WithUserId(entity.Property(o => o.User));
                HasCurrencyCodeConversion(entity.Property(e => e.Currency));
                entity.HasMany(e => e.Bids).WithOne()
                    .HasPrincipalKey(a=>a.AuctionId)
                    .HasForeignKey("AuctionId");
            });
        builder.Entity<TimedAscendingAuction>(entity =>
        {
            entity.HasDiscriminator(b => b.AuctionType).HasValue(AuctionType.TimedAscendingAuction);
            entity.OwnsOne<TimedAscendingOptions>(e=>e.Options);
        });
        builder.Entity<SingleSealedBidAuction>(entity =>
        {
            entity.HasDiscriminator(b => b.AuctionType).HasValue(AuctionType.SingleSealedBidAuction);
            entity.Property(e=>e.Options);
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