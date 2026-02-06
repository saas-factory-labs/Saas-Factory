using AppBlueprint.Domain.Entities.Notifications;
using AppBlueprint.Domain.Entities.Webhooks;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.DataExport;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.EntityConfigurations;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.FileManagement;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Partials.BaselineDbContext> _logger;

    // Public constructor for direct instantiation
    public BaselineDbContext(DbContextOptions<Partials.BaselineDbContext> options, IConfiguration configuration, ILogger<Partials.BaselineDbContext> logger)
        : this((DbContextOptions)options, configuration, logger)
    {
    }

    // Protected constructor for derived classes to pass their specific options
    protected BaselineDbContext(DbContextOptions options, IConfiguration configuration, ILogger<Partials.BaselineDbContext> logger)
        : base(options)
    {
        _configuration = configuration;
        _logger = logger;
        _logger.LogInformation("Baseline DbContext initialized");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<string>()
            .AreUnicode()
            .HaveMaxLength(1024);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // partial configurations
        OnModelCreating_Addressing(modelBuilder);
        OnModelCreating_Authorization(modelBuilder);
        OnModelCreating_Customers(modelBuilder);
        OnModelCreating_Audit(modelBuilder);
        OnModelCreating_Payment(modelBuilder);
        OnModelCreating_Permissions(modelBuilder);
        OnModelCreating_Users(modelBuilder);
        OnModelCreating_Tenants(modelBuilder);
        OnModelCreating_FileManagement(modelBuilder);

        modelBuilder.ApplyConfiguration(new Entities.EntityConfigurations.NotificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new LanguageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new WebhookEventEntityConfiguration());

        // User Notifications configurations
        modelBuilder.ApplyConfiguration(new Entities.EntityConfigurations.UserNotificationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Entities.EntityConfigurations.NotificationPreferencesEntityConfiguration());
        modelBuilder.ApplyConfiguration(new Entities.EntityConfigurations.PushNotificationTokenEntityConfiguration());

        HandleSensitiveData(modelBuilder);
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
                    // Basic Enum to String conversion
                    var type = typeof(Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToStringConverter<>)
                        .MakeGenericType(Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType);

                    var converter = Activator.CreateInstance(type) as Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter;
                    property.SetValueConverter(converter);
                }
            }
        }
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

    public DbSet<TenantEntity> Tenants { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<IntegrationEntity> Integrations { get; set; }
    public DbSet<LanguageEntity> Languages { get; set; }
    public DbSet<DataExportEntity> DataExports { get; set; }
    public DbSet<FileEntity> Files { get; set; }
    public DbSet<WebhookEntity> Webhooks { get; set; }
    public DbSet<WebhookEventEntity> WebhookEvents { get; set; }
    public DbSet<SearchEntity> Searches { get; set; }

    // User Notifications
    public DbSet<Domain.Entities.Notifications.UserNotificationEntity> UserNotifications { get; set; }
    public DbSet<Domain.Entities.Notifications.NotificationPreferencesEntity> NotificationPreferences { get; set; }
    public DbSet<Domain.Entities.Notifications.PushNotificationTokenEntity> PushNotificationTokens { get; set; }

    #endregion

    #region Partial methods

    partial void OnModelCreating_Addressing(ModelBuilder modelBuilder);
    partial void OnModelCreating_Audit(ModelBuilder modelBuilder);
    partial void OnModelCreating_Customers(ModelBuilder modelBuilder);
    partial void OnModelCreating_Payment(ModelBuilder modelBuilder);
    partial void OnModelCreating_Permissions(ModelBuilder modelBuilder);
    partial void OnModelCreating_Users(ModelBuilder modelBuilder);
    partial void OnModelCreating_Authorization(ModelBuilder modelBuilder);
    partial void OnModelCreating_Tenants(ModelBuilder modelBuilder);
    partial void OnModelCreating_FileManagement(ModelBuilder modelBuilder);

    #endregion
}
