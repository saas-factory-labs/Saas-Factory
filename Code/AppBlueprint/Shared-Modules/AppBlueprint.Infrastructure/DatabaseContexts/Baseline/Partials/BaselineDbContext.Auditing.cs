using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Auditing.ApiLog;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Auditing.AuditLog;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<AuditLogEntity> AuditLogs { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }

    partial void OnModelCreating_Audit(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApiLogEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SessionEntityConfiguration());
    }
}
