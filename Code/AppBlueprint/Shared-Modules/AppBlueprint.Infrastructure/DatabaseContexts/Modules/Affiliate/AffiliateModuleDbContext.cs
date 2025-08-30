using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Modules.Affiliate;

public partial class AffiliateModuleDbContext : ApplicationDbContext
{
    public AffiliateModuleDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AffiliateModuleDbContext> logger) :
        base(options, configuration, httpContextAccessor, logger)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Affiliate(modelBuilder);
    }

    partial void OnModelCreating_Affiliate(ModelBuilder modelBuilder);
}