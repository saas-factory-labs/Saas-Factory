using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BaselineDbContext = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Partials.BaselineDbContext;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

/// <summary>
/// Factory for creating BaselineDbContext instances at design time (for migrations).
/// </summary>
public sealed class BaselineDbContextFactory : IDesignTimeDbContextFactory<BaselineDbContext>
{
    /// <summary>
    /// Design-time factory method used by EF Core migrations.
    /// </summary>
    public BaselineDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING",
            EnvironmentVariableTarget.User);

        var optionsBuilder = new DbContextOptionsBuilder<BaselineDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.CommandTimeout(60)
        );

        // Suppress pending model changes warning for design-time migrations
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

        // Create a logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var designLogger = loggerFactory.CreateLogger<BaselineDbContext>();

        return new BaselineDbContext(optionsBuilder.Options, configuration, designLogger);
    }
}
