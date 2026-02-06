using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

/// <summary>
/// Design-time factory for creating ApplicationDbContext instances for EF Core migrations.
/// This class MUST have a parameterless constructor for EF Core design-time tools.
/// </summary>
public sealed class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Design-time factory method used by EF Core migrations.
    /// </summary>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING",
            EnvironmentVariableTarget.User);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.CommandTimeout(60)
        );

        // Suppress pending model changes warning for design-time migrations
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        // Create a logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var designLogger = loggerFactory.CreateLogger<ApplicationDbContext>();

        // Pass null for ITenantContextAccessor - migrations don't need tenant filtering
        return new ApplicationDbContext(optionsBuilder.Options, configuration, designLogger, null);
    }
}

/// <summary>
/// Runtime factory for creating ApplicationDbContext instances.
/// Used by application services via dependency injection.
/// </summary>
public sealed class ApplicationDbContextFactory(
    DbContextOptions<ApplicationDbContext> options,
    IConfiguration configuration,
    ILogger<ApplicationDbContext> logger)
    : IDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Runtime factory method used by application services.
    /// Explicitly passes null for ITenantContextAccessor to avoid scoped service resolution from singleton factory.
    /// </summary>
    public ApplicationDbContext CreateDbContext()
    {
        // Pass null for ITenantContextAccessor - factory-created contexts don't have tenant filtering
        return new ApplicationDbContext(options, configuration, logger, tenantContextAccessor: null);
    }

    /// <summary>
    /// Async runtime factory method used by application services.
    /// </summary>
    public Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(CreateDbContext());
    }
}
