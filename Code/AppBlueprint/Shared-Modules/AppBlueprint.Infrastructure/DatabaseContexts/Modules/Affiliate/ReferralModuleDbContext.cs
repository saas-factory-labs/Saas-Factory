using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Appblueprint.SharedKernel.DatabaseContexts.Modules.Affiliate;

public partial class AffiliateModuleDbContext : ApplicationDbContext
{
    private readonly IConfiguration _configuration;

    public AffiliateModuleDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<B2CdbContext> logger) :
        base(options, configuration, httpContextAccessor, logger)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Affiliate(modelBuilder);
    }

    partial void OnModelCreating_Affiliate(ModelBuilder modelBuilder);
}
