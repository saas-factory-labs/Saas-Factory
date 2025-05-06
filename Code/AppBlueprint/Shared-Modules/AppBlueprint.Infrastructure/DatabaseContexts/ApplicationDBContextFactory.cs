using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AppBlueprint.Infrastructure.DatabaseContexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Optional, for appsettings.json
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        // supabase
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
        
        return new ApplicationDbContext(optionsBuilder.Options, configuration, httpContextAccessor);
    }
}
