using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Integration.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public BaselineDbContext(DbContextOptions options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<string>()
            .AreUnicode()
            .HaveMaxLength(1024);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // partial configurations
        OnModelCreating_Addressing(modelBuilder);
        OnModelCreating_Authorization(modelBuilder);
        OnModelCreating_Customers(modelBuilder);
        OnModelCreating_Audit(modelBuilder);
        OnModelCreating_Payment(modelBuilder);
        OnModelCreating_Permissions(modelBuilder);
        OnModelCreating_Users(modelBuilder);

        modelBuilder.ApplyConfiguration(new NotificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageEntityConfiguration());

        HandleSensitiveData(modelBuilder);
    }

    private void HandleSensitiveData(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        foreach (var property in entityType.GetProperties())
        {
            var isSensitive = property.PropertyInfo?
                .GetCustomAttributes(typeof(SensitiveDataAttribute), false)
                .FirstOrDefault();

            if (isSensitive is not null)
                property.SetAnnotation("SensitiveData", true);
        }
    }

    #region DbSets

    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<IntegrationEntity> Integrations { get; set; }
    public DbSet<LanguageEntity> Languages { get; set; }
    public DbSet<DataExportEntity> DataExports { get; set; }
    public DbSet<FileEntity> Files { get; set; }

    #endregion

    #region Partial methods

    partial void OnModelCreating_Addressing(ModelBuilder modelBuilder);
    partial void OnModelCreating_Audit(ModelBuilder modelBuilder);
    partial void OnModelCreating_Customers(ModelBuilder modelBuilder);
    partial void OnModelCreating_Payment(ModelBuilder modelBuilder);
    partial void OnModelCreating_Permissions(ModelBuilder modelBuilder);
    partial void OnModelCreating_Users(ModelBuilder modelBuilder);
    partial void OnModelCreating_Authorization(ModelBuilder modelBuilder);

    #endregion
}
