using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Wallymathieu.Auctions.Infrastructure.Data;

public class AuctionDbContext: DbContext, IRepository<Auction>
{
    public AuctionDbContext()
    {
    }

    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
    {
    }

    public DbSet<Auction> Auctions { get; set; }

    private static PropertyBuilder<T> WithAuctionIdConversion<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<AuctionId, long>(v => v.Id, v => new AuctionId(v)));

    private static PropertyBuilder<T> WithUserId<T>(PropertyBuilder<T> self) =>
        self.HasConversion(new ValueConverter<UserId, string>(v => v.Id!, v => new UserId(v))).HasMaxLength(2000);

    private static PropertyBuilder<CurrencyCode> HasCurrencyCodeConversion(
        PropertyBuilder<CurrencyCode> propertyBuilder) =>
        propertyBuilder.HasConversion(new EnumToStringConverter<CurrencyCode>()).HasMaxLength(3);

    /// <summary>
    /// Returns a list of auctions that are not tracked by Entity Framework Core.
    /// </summary>
    public async ValueTask<IReadOnlyCollection<Auction>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        return await Auctions.AsNoTracking()
            .Include("Bids")
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Get a tracked auction aggregate if found (that it is tracked by Entity Framework Core). This will also load the bids.
    /// </summary>
    public async ValueTask<Auction?> GetAuction(AuctionId auctionId, CancellationToken cancellationToken = default)
    {
        var auction = await Auctions.FindAsync(keyValues: [auctionId], cancellationToken: cancellationToken);
        if (auction is not null) await Entry(auction).Collection("Bids").LoadAsync(cancellationToken);
        return auction;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<Auction>(entity =>
        {
            entity.HasDiscriminator(b => b.AuctionType).IsComplete(false);
            entity.ToTable("Auctions");
            entity.HasKey(e => e.AuctionId);
            WithAuctionIdConversion(entity.Property(e => e.AuctionId).ValueGeneratedOnAdd());
            entity.Property(e => e.Title).HasMaxLength(200);
            WithUserId(entity.Property(o => o.User));
            HasCurrencyCodeConversion(entity.Property(e => e.Currency));
            entity.Property(p => p.Version).IsConcurrencyToken();
            entity.HasMany("Bids").WithOne()
                .HasPrincipalKey("AuctionId")
                .HasForeignKey("AuctionId");
        });
        modelBuilder.Entity<TimedAscendingAuction>(entity =>
        {
            entity.HasDiscriminator(b => b.AuctionType).HasValue(AuctionType.TimedAscendingAuction);
            entity.OwnsOne(e => e.Options);
        });
        modelBuilder.Entity<SingleSealedBidAuction>(entity =>
        {
            entity.HasDiscriminator(b => b.AuctionType).HasValue(AuctionType.SingleSealedBidAuction);
            entity.Property(e => e.Options);
        });

        modelBuilder.Entity<BidEntity>(entity =>
        {
            entity.ToTable("Bids");
            WithUserId(entity.Property(o => o.User));
        });


        base.OnModelCreating(modelBuilder);
    }

    async ValueTask IRepository<Auction>.AddAsync(Auction entity, CancellationToken cancellationToken)
    {
        await Auctions.AddAsync(entity, cancellationToken);
    }

    async Task<Auction?> IRepository<Auction>.FindAsync(object identifier, CancellationToken cancellationToken)
    {
        return await Auctions.FindAsync(identifier, cancellationToken);
    }
}