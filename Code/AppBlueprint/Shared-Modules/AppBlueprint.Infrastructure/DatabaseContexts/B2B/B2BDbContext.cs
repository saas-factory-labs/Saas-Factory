using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrganizationEntity = AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization.OrganizationEntity;
using OrganizationEntityConfiguration = AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization.OrganizationEntityConfiguration;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public partial class B2BDbContext : BaselineDbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string? _connectionString;

    public B2BDbContext(
        DbContextOptions<B2BDbContext> options,
        IConfiguration configuration,
        ILogger<B2BDbContext> logger)
        : base((DbContextOptions)options, configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;
        // Try multiple connection string names, don't throw if not found
        // The options passed in already have the connection configured from DI
        _connectionString = configuration.GetConnectionString("appblueprintdb")
                           ?? configuration.GetConnectionString("postgres-server")
                           ?? configuration.GetConnectionString("DefaultConnection");
    }

    protected B2BDbContext(
        DbContextOptions options,
        IConfiguration configuration,
        ILogger logger)
        : base(options, configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("appblueprintdb")
                           ?? configuration.GetConnectionString("postgres-server")
                           ?? configuration.GetConnectionString("DefaultConnection");
    }

    public DbSet<ApiKeyEntity> ApiKeys { get; set; }
    public DbSet<OrganizationEntity> Organizations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            _logger.LogInformation("B2B DbContext configured with connection string");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApiKeyEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrganizationEntityConfiguration());

        OnModelCreating_Tenant(modelBuilder);
        OnModelCreating_Team(modelBuilder);
        OnModelCreating_Todo(modelBuilder);
    }

    partial void OnModelCreating_Tenant(ModelBuilder modelBuilder);
    partial void OnModelCreating_Team(ModelBuilder modelBuilder);
    partial void OnModelCreating_Todo(ModelBuilder modelBuilder);
}
