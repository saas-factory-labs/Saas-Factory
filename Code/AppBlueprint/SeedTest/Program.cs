using AppBlueprint.Infrastructure;
using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.SeedTest;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🌱 Starting Database Seeding Test...");

        var connectionString = Environment.GetEnvironmentVariable("APPBLUEPRINT_DATABASE_CONNECTIONSTRING")
                              ?? "Host=localhost;Port=63408;Database=appblueprintdb;Username=postgres;Password=password";

        Console.WriteLine($"Using connection string: {connectionString.Replace("Password=password", "Password=***", StringComparison.Ordinal)}");

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
            var counts = new Dictionary<string, int>();

            // Check core tables
            counts["Languages"] = await dbContext.Languages.CountAsync();
            counts["Countries"] = await dbContext.Countries.CountAsync();
            counts["GlobalRegions"] = await dbContext.GlobalRegions.CountAsync();
            counts["Addresses"] = await dbContext.Addresses.CountAsync();
            counts["Users"] = await dbContext.Users.CountAsync();
            counts["Tenants"] = await dbContext.Tenants.CountAsync();
            counts["Customers"] = await dbContext.Customers.CountAsync();
            counts["EmailAddresses"] = await dbContext.EmailAddresses.CountAsync();
            counts["Roles"] = await dbContext.Roles.CountAsync();
            counts["Permissions"] = await dbContext.Permissions.CountAsync();
            counts["Teams"] = await b2bDbContext.Teams.CountAsync();
            counts["AuditLogs"] = await dbContext.AuditLogs.CountAsync();

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
