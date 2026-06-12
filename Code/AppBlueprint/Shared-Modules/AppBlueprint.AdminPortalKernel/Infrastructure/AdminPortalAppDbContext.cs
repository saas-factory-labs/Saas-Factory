using AppBlueprint.AdminPortalKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Thin EF context over a target app's own database, mapping only the baseline
/// "Users"/"Tenants" tables. Deliberately NOT BaselineDbContext: that would couple the
/// admin portal to every app's full schema version. This context never creates
/// migrations - the target app owns its schema.
/// </summary>
public sealed class AdminPortalAppDbContext : DbContext
{
    public AdminPortalAppDbContext(DbContextOptions<AdminPortalAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdminUserRecord> Users => Set<AdminUserRecord>();

    public DbSet<AdminTenantRecord> Tenants => Set<AdminTenantRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AdminUserRecord>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.HasQueryFilter(user => !user.IsSoftDeleted);
        });

        modelBuilder.Entity<AdminTenantRecord>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(tenant => tenant.Id);
            entity.HasQueryFilter(tenant => !tenant.IsSoftDeleted);
        });
    }
}
