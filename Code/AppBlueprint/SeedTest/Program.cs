using System.Text.RegularExpressions;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.SeedTest;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🌱 Starting Database Seeding Test...");

        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTIONSTRING")
            ?? throw new InvalidOperationException("DATABASE_CONNECTIONSTRING environment variable is not set.");

        Console.WriteLine($"Using connection string: {Regex.Replace(connectionString, "Password=[^;]+", match => match.Value[..match.Value.IndexOf('=')] + "=***", RegexOptions.IgnoreCase)}");

        // Create host and services
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add logging
                services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

                // Add HTTP context accessor
                services.AddHttpContextAccessor();

                // Add database contexts
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

                services.AddDbContext<B2BDbContext>(options =>
                    options.UseNpgsql(connectionString));
            })
            .Build();

        try
        {
            using var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var b2bDbContext = scope.ServiceProvider.GetRequiredService<B2BDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

            Console.WriteLine("✅ Database contexts created successfully");

            // Test database connection
            if (await dbContext.Database.CanConnectAsync())
            {
                Console.WriteLine("✅ Database connection successful");

                // Get pending migrations
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                Console.WriteLine($"📋 Pending migrations: {pendingMigrations.Count()}");

                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"   - {migration}");
                }

                // Apply migrations if needed
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("🔄 Applying migrations...");
                    await dbContext.Database.MigrateAsync();
                    Console.WriteLine("✅ Migrations applied successfully");
                }

                // Check table counts before seeding
                Console.WriteLine("\n📊 Table counts before seeding:");
                await PrintTableCounts(dbContext, b2bDbContext);

                // Run seeding
                Console.WriteLine("\n🌱 Starting database seeding...");
                var dataSeeder = new DataSeeder(dbContext, b2bDbContext, logger);
                await dataSeeder.SeedDatabaseAsync();
                Console.WriteLine("✅ Database seeding completed");

                // Check table counts after seeding
                Console.WriteLine("\n📊 Table counts after seeding:");
                await PrintTableCounts(dbContext, b2bDbContext);
            }
            else
            {
                Console.WriteLine("❌ Cannot connect to database");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("🏁 Seeding test completed");
    }

    private static async Task PrintTableCounts(ApplicationDbContext dbContext, B2BDbContext b2bDbContext)
    {
        try
        {
            var counts = new Dictionary<string, int>
            {
                // Check core tables
                ["Languages"] = await dbContext.Languages.CountAsync(),
                ["Countries"] = await dbContext.Countries.CountAsync(),
                ["GlobalRegions"] = await dbContext.GlobalRegions.CountAsync(),
                ["Addresses"] = await dbContext.Addresses.CountAsync(),
                ["Users"] = await dbContext.Users.CountAsync(),
                ["Tenants"] = await dbContext.Tenants.CountAsync(),
                ["Customers"] = await dbContext.Customers.CountAsync(),
                ["EmailAddresses"] = await dbContext.EmailAddresses.CountAsync(),
                ["Roles"] = await dbContext.Roles.CountAsync(),
                ["Permissions"] = await dbContext.Permissions.CountAsync(),
                ["Teams"] = await b2bDbContext.Teams.CountAsync(),
                ["AuditLogs"] = await dbContext.AuditLogs.CountAsync()
            };

            foreach (var kvp in counts)
            {
                Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ⚠️  Error getting table counts: {ex.Message}");
        }
    }
}
