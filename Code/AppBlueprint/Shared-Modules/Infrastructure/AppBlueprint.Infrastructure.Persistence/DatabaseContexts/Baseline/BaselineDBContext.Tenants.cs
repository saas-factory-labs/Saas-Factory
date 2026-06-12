using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    partial void OnModelCreating_Tenants(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
    }
}
