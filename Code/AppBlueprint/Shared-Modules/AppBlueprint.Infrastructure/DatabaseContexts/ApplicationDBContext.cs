using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2C;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContext : B2CdbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly int _tenantId = 1; // Replace with tenantProvider.GetTenantId() when ready

    public ApplicationDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor
    ) : base(options, configuration)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Optional: You can add fallback logic or logging here
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mark properties with [GDPRType] as sensitive
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                var sensitiveDataAttribute = property.PropertyInfo?
                    .GetCustomAttributes(typeof(GDPRType), false)
                    .FirstOrDefault();

                if (sensitiveDataAttribute != null)
                {
                    property.SetAnnotation("SensitiveData", true);
                }
            }
        }

        // Optional: add multi-tenancy or soft-delete query filters here when ready
    }
}
