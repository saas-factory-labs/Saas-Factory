using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate;

public partial class AffiliateModuleDbContext : ApplicationDbContext
{
    public AffiliateModuleDbContext(
        DbContextOptions<AffiliateModuleDbContext> options,
        IConfiguration configuration,
        ILogger<AffiliateModuleDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null) :
        base((DbContextOptions)(DbContextOptions<AffiliateModuleDbContext>)options, configuration, logger, tenantContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Affiliate(modelBuilder);
    }

    partial void OnModelCreating_Affiliate(ModelBuilder modelBuilder);
}
