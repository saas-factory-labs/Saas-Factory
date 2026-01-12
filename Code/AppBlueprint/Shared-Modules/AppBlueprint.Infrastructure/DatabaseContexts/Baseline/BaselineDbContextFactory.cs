using AppBlueprint.Infrastructure.DatabaseContexts.Baseline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public sealed class BaselineDbContextFactory : IDesignTimeDbContextFactory<BaselineDbContext>
{
    public BaselineDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        string? connectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING",
            EnvironmentVariableTarget.User);

        // Fallback to configuration if environment variable not set
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb")
                ?? configuration.GetConnectionString("postgres-server")
                ?? configuration.GetConnectionString("DefaultConnection");
        }

        var optionsBuilder = new DbContextOptionsBuilder<BaselineDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.CommandTimeout(60)
        );

        // Create logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<BaselineDbContext>();

        return new BaselineDbContext(optionsBuilder.Options, configuration, logger);
    }
}
