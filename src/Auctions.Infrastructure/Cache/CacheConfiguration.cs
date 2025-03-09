namespace Wallymathieu.Auctions.Infrastructure.Cache;

public class CacheConfiguration
{
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(20);
}
