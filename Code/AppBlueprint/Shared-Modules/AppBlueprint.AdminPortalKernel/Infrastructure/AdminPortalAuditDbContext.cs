using AppBlueprint.AdminPortalKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Standalone context over DeploymentManager's database mapping ONLY dm_admin_audit,
/// so DeploymentManager.Web can read/write audit entries without referencing the
/// ApiService project. The table schema is owned by DeploymentManagerDbContext's
/// migrations (via ConfigureAdminPortalKernel) - this context never migrates.
/// </summary>
public sealed class AdminPortalAuditDbContext : DbContext
{
    public AdminPortalAuditDbContext(DbContextOptions<AdminPortalAuditDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdminAuditEntryEntity> AuditEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new AdminAuditEntryConfiguration());
    }
}
