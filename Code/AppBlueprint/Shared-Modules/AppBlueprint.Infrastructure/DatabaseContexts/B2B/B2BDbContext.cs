using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public partial class B2BDbContext : BaselineDbContext
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<B2BDbContext> _logger;
    private readonly string _connectionString;

    public B2BDbContext(DbContextOptions options, IConfiguration configuration, ILogger<B2BDbContext> logger)
        : base(options, configuration, logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("SUPABASE_POSTGRESQL")
                           ?? throw new InvalidOperationException("Missing connection string.");
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

        OnModelCreating_Team(modelBuilder);
        OnModelCreating_Tenant(modelBuilder);
        OnModelCreating_Todo(modelBuilder);
    }

    partial void OnModelCreating_Tenant(ModelBuilder modelBuilder);
    partial void OnModelCreating_Team(ModelBuilder modelBuilder);
    partial void OnModelCreating_Todo(ModelBuilder modelBuilder);
}
