using Auctions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.Data;

public class AuctionDbContext: DbContext
{
    public AuctionDbContext()
    {
    }
    public DbSet<TimedAscendingAuction> Auctions { get; set; }
    private PropertyBuilder<T> WithAuctionIdConversion<T>(PropertyBuilder<T> self) => self.HasConversion(new ValueConverter<AuctionId, long>(v => v.Id, v => new AuctionId(v)));
    private PropertyBuilder<T> WithUserId<T>(PropertyBuilder<T> self) => self.HasConversion(new ValueConverter<UserId, string>(v => v.Id, v => new UserId(v))).HasMaxLength(2000);
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
            entity.HasMany(e => e.Bids).WithOne();
        });
        
        builder.Entity<BidEntity>(entity =>
        {
            entity.ToTable("Bids");
            WithUserId(entity.Property(o => o.User));
            entity.OwnsOne(e => e.Amount);
        });
        
       
        base.OnModelCreating(builder);
    }
}