using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
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

        // Create a mock HttpContextAccessor for design-time
        var httpContextAccessor = new HttpContextAccessor();
        
        // Create a logger factory for design-time
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<ApplicationDbContext>();

        return new ApplicationDbContext(optionsBuilder.Options, configuration, httpContextAccessor, logger);
    }
}
