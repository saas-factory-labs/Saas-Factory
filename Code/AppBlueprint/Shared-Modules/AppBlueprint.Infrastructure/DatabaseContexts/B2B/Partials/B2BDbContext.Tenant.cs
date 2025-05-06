using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public partial class B2BDbContext
{
    public DbSet<TenantEntity> Tenants { get; set; }

    partial void OnModelCreating_Tenant(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
    }
}
