using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C;

public partial class B2CdbContext : BaselineDbContext
{
    private readonly string? _connectionString;
    private readonly ILogger<B2CdbContext> _logger;

    // Public constructor for direct instantiation
    public B2CdbContext(DbContextOptions<B2CdbContext> options, IConfiguration configuration, ILogger<B2CdbContext> logger)
        : this((DbContextOptions)options, configuration, logger)
    {
    }

    // Protected constructor for derived classes
    protected B2CdbContext(DbContextOptions options, IConfiguration configuration, ILogger<B2CdbContext> logger)
        : base(options, configuration, logger)
    {
        // Try to get connection string, but don't throw if it's not found
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);
        
        // Only configure using _connectionString if:
        // 1. The options aren't already configured, AND
        // 2. We have a valid connection string
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
            
            // Simple logging pattern - warning suppressed in .editorconfig
            _logger.LogInformation("B2C DbContext configured with connection string");
        }
        // If options are already configured or we don't have a connection string,
        // rely on the options provided in the constructor
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreating_Family(modelBuilder);
    }

    partial void OnModelCreating_Family(ModelBuilder modelBuilder);
}
