using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2C;

public partial class B2CdbContext : BaselineDbContext
{
    private readonly string? _connectionString;

    public B2CdbContext(DbContextOptions options, IConfiguration configuration)
        : base(options, configuration)
    {
        // Try to get connection string, but don't throw if it's not found
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure using _connectionString if:
        // 1. The options aren't already configured, AND
        // 2. We have a valid connection string
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
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
