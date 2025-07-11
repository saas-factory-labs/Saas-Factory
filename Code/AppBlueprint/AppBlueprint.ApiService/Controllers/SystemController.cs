using AppBlueprint.Infrastructure;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace AppBlueprint.ApiService.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/system")]
internal class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SystemController> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public SystemController(
        ApplicationDbContext dbContext,
        ILogger<SystemController> logger,
        ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _dbContext = dbContext;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> ApplyMigrations()
    {
        try
        {
            _logger.LogInformation("Manually applying database migrations...");

            // Check database connection
            if (await _dbContext.Database.CanConnectAsync())
            {
                await _dbContext.Database.MigrateAsync();
                _logger.LogInformation("Migrations applied successfully");

                // Call the seeder as well
                var dataSeederLogger = _loggerFactory.CreateLogger<DataSeeder>();
                var dataSeeder = new DataSeeder(_dbContext, dataSeederLogger);
                await dataSeeder.SeedDatabaseAsync();
                _logger.LogInformation("Database seeding completed successfully");

                return Ok(new { success = true, message = "Migrations applied successfully" });
            }
            else
            {
                _logger.LogWarning("Cannot connect to the database");
                return StatusCode(500, new { success = false, message = "Cannot connect to the database" });
            }
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Database error while applying migrations");
            return StatusCode(500, new { success = false, message = $"Database error applying migrations: {ex.Message}" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Entity Framework error while applying migrations");
            return StatusCode(500, new { success = false, message = $"EF Core error applying migrations: {ex.Message}" });
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout error while applying migrations");
            return StatusCode(500, new { success = false, message = $"Timeout error applying migrations: {ex.Message}" });
        }
    }

    [HttpGet("db-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            var connectionString = _dbContext.Database.GetConnectionString();

            // Mask the password in the connection string for security
            var maskedConnectionString = connectionString?.Replace(";Password=", ";Password=*****", StringComparison.OrdinalIgnoreCase);

            object response;

            if (canConnect)
            {
                // Check if there are pending migrations
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                response = new
                {
                    canConnect,
                    connectionString = maskedConnectionString,
                    migrationsPending = pendingMigrations.Any(),
                    pendingMigrationCount = pendingMigrations.Count()
                };
            }
            else
            {
                response = new
                {
                    canConnect,
                    connectionString = maskedConnectionString,
                    migrationsPending = false
                };
            }

            return Ok(response);
        }
        catch (NpgsqlException ex)
        {
            return StatusCode(500, new { success = false, message = $"Database error checking status: {ex.Message}" });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { success = false, message = $"EF Core error checking database status: {ex.Message}" });
        }
        catch (TimeoutException ex)
        {
            return StatusCode(500, new { success = false, message = $"Timeout error checking database status: {ex.Message}" });
        }
    }
}