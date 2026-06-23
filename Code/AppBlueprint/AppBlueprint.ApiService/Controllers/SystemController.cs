using AppBlueprint.Application.Constants;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// ReSharper disable LocalizableElement
namespace AppBlueprint.ApiService.Controllers;

[Authorize(Roles = Roles.DeploymentManagerAdmin)]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/system")]
internal class SystemController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly B2BDbContext _b2bDbContext;
    private readonly ILogger<SystemController> _logger;

    public SystemController(
        ApplicationDbContext dbContext,
        B2BDbContext b2bDbContext,
        ILogger<SystemController> logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(b2bDbContext);
        ArgumentNullException.ThrowIfNull(logger);

        _dbContext = dbContext;
        _b2bDbContext = b2bDbContext;
        _logger = logger;
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> ApplyMigrations()
    {
        try
        {
            _logger.LogInformation("Manually applying database migrations...");

            if (!await _dbContext.Database.CanConnectAsync())
            {
                _logger.LogWarning("Cannot connect to the database");
                return StatusCode(500, new { success = false, message = "Cannot connect to the database" });
            }

            await _dbContext.Database.MigrateAsync();
            await _b2bDbContext.Database.MigrateAsync();
            _logger.LogInformation("Database migrations applied successfully");

            return Ok(new { success = true, message = "Migrations applied successfully" });
        }
        catch (NpgsqlException ex)
        {
            _logger.LogError(ex, "Database error while applying migrations");
            return StatusCode(500, new { success = false, message = "Database error applying migrations. Please check logs for details." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Entity Framework error while applying migrations");
            return StatusCode(500, new { success = false, message = "EF Core error applying migrations. Please check logs for details." });
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout error while applying migrations");
            return StatusCode(500, new { success = false, message = "Timeout error applying migrations. Please check logs for details." });
        }
    }

    [HttpGet("db-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();

            object response;

            if (canConnect)
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                response = new
                {
                    canConnect,
                    migrationsPending = pendingMigrations.Any(),
                    pendingMigrationCount = pendingMigrations.Count()
                };
            }
            else
            {
                response = new
                {
                    canConnect,
                    migrationsPending = false
                };
            }

            return Ok(response);
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"Database error checking status: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Database error checking status. Please check logs for details." });
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"EF Core error checking database status: {ex.Message}");
            return StatusCode(500, new { success = false, message = "EF Core error checking database status. Please check logs for details." });
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout error checking database status: {ex.Message}");
            return StatusCode(500, new { success = false, message = "Timeout error checking database status. Please check logs for details." });
        }
    }
}
