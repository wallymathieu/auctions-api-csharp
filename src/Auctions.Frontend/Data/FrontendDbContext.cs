using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wallymathieu.Auctions.Frontend.Data;

public class FrontendDbContext : IdentityDbContext
{
    public FrontendDbContext(DbContextOptions<FrontendDbContext> options)
        : base(options)
    {
    }
}
