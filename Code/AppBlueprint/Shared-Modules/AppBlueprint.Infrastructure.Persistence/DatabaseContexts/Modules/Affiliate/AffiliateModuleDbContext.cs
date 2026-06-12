using AppBlueprint.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Modules.Affiliate;

public partial class AffiliateModuleDbContext : ApplicationDbContext
{
    public AffiliateModuleDbContext(
        DbContextOptions<AffiliateModuleDbContext> options,
        IConfiguration configuration,
        ILogger<AffiliateModuleDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null) :
        base((DbContextOptions)options, configuration, logger, tenantContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Affiliate(modelBuilder);
    }

    partial void OnModelCreating_Affiliate(ModelBuilder modelBuilder);
}
