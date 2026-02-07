using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts.B2B;

public sealed class B2BDbContextFactory : IDesignTimeDbContextFactory<B2BDbContext>
{
    public B2BDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .Build();

        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING",
            EnvironmentVariableTarget.User);

        // Fallback to configuration if environment variable not set
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = configuration.GetConnectionString("appblueprintdb")
                ?? configuration.GetConnectionString("postgres-server")
                ?? configuration.GetConnectionString("DefaultConnection");
        }

        var optionsBuilder = new DbContextOptionsBuilder<B2BDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions => npgsqlOptions.CommandTimeout(60)
        );

        // Create logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<B2BDbContext>();

        return new B2BDbContext(optionsBuilder.Options, configuration, logger);
    }
}
