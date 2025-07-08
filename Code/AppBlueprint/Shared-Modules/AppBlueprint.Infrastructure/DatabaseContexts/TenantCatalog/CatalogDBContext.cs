using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog;

public class CatalogDbContext : DbContext
{
    public DbSet<TenantEntity> Tenants { get; set; }
    public DbSet<CustomerEntity> Customers { get; set; }


    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        // default constraints
        configurationBuilder.Properties<string>()
            .AreUnicode()
            .HaveMaxLength(1024); // can be overriden in the entity configuration
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new AppProjectEntityConfiguration());
    }
}
