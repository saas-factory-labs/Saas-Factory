using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog;

public class CatalogDbContext : DbContext
{
    public DbSet<TenantEntity> Tenants { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }


    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ILogger<CatalogDbContext> logger)
        : base(options)
    {
        var _logger = logger;
        _logger.LogInformation("Catalog DbContext initialized");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);
        // default constraints
        configurationBuilder.Properties<string>()
            .AreUnicode()
            .HaveMaxLength(1024); // can be overriden in the entity configuration
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new AppProjectEntityConfiguration());

        HandleEnumToStringConversion(modelBuilder);
    }

    private static void HandleEnumToStringConversion(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum || (Nullable.GetUnderlyingType(property.ClrType)?.IsEnum ?? false))
                {
                    var type = typeof(Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToStringConverter<>)
                        .MakeGenericType(Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType);

                    var converter = Activator.CreateInstance(type) as Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter;
                    property.SetValueConverter(converter);
                }
            }
        }
    }
}
