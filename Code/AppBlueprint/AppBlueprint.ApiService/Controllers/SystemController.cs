using AppBlueprint.Infrastructure;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
internal class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ApplicationDbContext dbContext,
        ILogger<SystemController> logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);
        
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> ApplyMigrations()
    {
        try
        {
            _logger.LogInformation("Manually applying database migrations...");
            
            // Check database connection
            if (await _dbContext.Database.CanConnectAsync().ConfigureAwait(false))
            {
                await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
                _logger.LogInformation("Migrations applied successfully");
                
                // Call the seeder as well
                var dataSeeder = new DataSeeder(_dbContext);
                await dataSeeder.SeedDatabaseAsync().ConfigureAwait(false);
                _logger.LogInformation("Database seeding completed successfully");
                
                return Ok(new { success = true, message = "Migrations applied successfully" });
            }
            else
            {
                _logger.LogWarning("Cannot connect to the database");
                return StatusCode(500, new { success = false, message = "Cannot connect to the database" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migrations");
            return StatusCode(500, new { success = false, message = $"Error applying migrations: {ex.Message}" });
        }
    }

    [HttpGet("db-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync().ConfigureAwait(false);
            var connectionString = _dbContext.Database.GetConnectionString();
            
            // Mask the password in the connection string for security
            var maskedConnectionString = connectionString?.Replace(";Password=", ";Password=*****");
            
            object response;
            
            if (canConnect)
            {
                // Check if there are pending migrations
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync().ConfigureAwait(false);
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
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Error checking database status: {ex.Message}" });
        }
    }
}