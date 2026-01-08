using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

/// <summary>
/// Factory for creating ApplicationDbContext instances.
/// Implements both design-time (for migrations) and runtime factory interfaces.
/// </summary>
public sealed class ApplicationDbContextFactory(
    DbContextOptions<ApplicationDbContext> options,
    IConfiguration configuration,
    ILogger<ApplicationDbContext> logger)
    : IDesignTimeDbContextFactory<ApplicationDbContext>, IDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Design-time factory method used by EF Core migrations.
    /// </summary>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Optional, for appsettings.json
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        string? connectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING",
            EnvironmentVariableTarget.User);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Specify the overload explicitly
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.CommandTimeout(60)
        );

        // Create a logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var designLogger = loggerFactory.CreateLogger<ApplicationDbContext>();

        // Pass null for ITenantContextAccessor - migrations don't need tenant filtering
        return new ApplicationDbContext(optionsBuilder.Options, configuration, designLogger, null);
    }

    /// <summary>
    /// Runtime factory method used by application services.
    /// Explicitly passes null for ITenantContextAccessor to avoid scoped service resolution from singleton factory.
    /// </summary>
    public ApplicationDbContext CreateDbContext()
    {
        // Pass null for ITenantContextAccessor - factory-created contexts don't have tenant filtering
        return new ApplicationDbContext(options, configuration, logger, tenantContextAccessor: null);
    }
}