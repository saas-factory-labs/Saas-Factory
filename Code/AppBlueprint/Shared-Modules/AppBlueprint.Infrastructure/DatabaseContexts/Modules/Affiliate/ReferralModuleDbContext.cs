using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Referral;

public partial class ReferralModuleDbContext : ApplicationDbContext
{
    public ReferralModuleDbContext(
        DbContextOptions<ReferralModuleDbContext> options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ReferralModuleDbContext> logger) :
        base((DbContextOptions)(DbContextOptions<ReferralModuleDbContext>)options, configuration, httpContextAccessor, logger)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Referral(modelBuilder);
    }

    partial void OnModelCreating_Referral(ModelBuilder modelBuilder);
}
