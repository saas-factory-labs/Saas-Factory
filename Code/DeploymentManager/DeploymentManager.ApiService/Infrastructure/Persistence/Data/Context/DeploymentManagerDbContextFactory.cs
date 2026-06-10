using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;

public class DeploymentManagerDbContextFactory : IDesignTimeDbContextFactory<DeploymentManagerDbContext>
{
    public DeploymentManagerDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<DeploymentManagerDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = PostgresConnectionStringHelper.Normalize(
            configuration.GetConnectionString("DefaultConnection")
            ?? configuration["DATABASE_URL"]
            ?? throw new InvalidOperationException(
                "No database connection string found. Set ConnectionStrings:DefaultConnection via user secrets or DATABASE_URL env var."));

        DbContextOptionsBuilder<DeploymentManagerDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(connectionString);

        ILoggerFactory loggerFactory = LoggerFactory.Create(b => b.AddConsole());

        return new DeploymentManagerDbContext(
            optionsBuilder.Options,
            configuration,
            loggerFactory.CreateLogger<DeploymentManagerDbContext>());
    }
}
