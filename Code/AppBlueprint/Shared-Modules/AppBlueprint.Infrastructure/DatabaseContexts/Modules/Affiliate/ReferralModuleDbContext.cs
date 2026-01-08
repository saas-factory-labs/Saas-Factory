using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Referral;

public partial class ReferralModuleDbContext : ApplicationDbContext
{
    public ReferralModuleDbContext(
        DbContextOptions<ReferralModuleDbContext> options,
        IConfiguration configuration,
        ILogger<ReferralModuleDbContext> logger,
        ITenantContextAccessor? tenantContextAccessor = null) :
        base((DbContextOptions)(DbContextOptions<ReferralModuleDbContext>)options, configuration, logger, tenantContextAccessor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Referral(modelBuilder);
    }

    partial void OnModelCreating_Referral(ModelBuilder modelBuilder);
}
