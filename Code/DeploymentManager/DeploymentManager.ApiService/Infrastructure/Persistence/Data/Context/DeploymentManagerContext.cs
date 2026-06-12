using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Baseline;
using DeploymentManager.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;

public class DeploymentManagerDbContext : BaselineDbContext
{
    public DeploymentManagerDbContext(
        DbContextOptions<DeploymentManagerDbContext> options,
        IConfiguration configuration,
        ILogger<DeploymentManagerDbContext> logger)
        : base(options, configuration, logger)
    {
    }

    public DbSet<AppEntity> DmApps { get; set; }
    public DbSet<CustomerEntity> DmCustomers { get; set; }
    public DbSet<DeploymentEntity> DmDeployments { get; set; }
    public DbSet<ProjectEntity> DmProjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppEntity>().ToTable("dm_apps");
        modelBuilder.Entity<CustomerEntity>().ToTable("dm_customers");
        modelBuilder.Entity<DeploymentEntity>().ToTable("dm_deployments");
        modelBuilder.Entity<ProjectEntity>().ToTable("dm_projects");
    }
}
