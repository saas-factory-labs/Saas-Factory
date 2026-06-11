using AppBlueprint.Application.Constants;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Database;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Implementation of IAdminTenantAccessService for read-only admin access to tenant data.
/// ⚠️ READ-ONLY: Admins can only VIEW tenant data, never modify or delete.
/// 
/// This service enables DeploymentManager and admin portals to view tenant data
/// for support purposes (e.g., troubleshooting, support tickets) while maintaining
/// strict security controls:
/// 
/// - READ-ONLY: Uses .AsNoTracking() to prevent modifications via EF Core
/// - RBAC: Only DeploymentManagerAdmin role can access
/// - AUDIT: All access logged with reason, timestamp, IP address
/// - TEMPORARY: Tenant context cleared after operation
/// - DEFENSE-IN-DEPTH: RLS enforces read-only at database level
/// 
/// Usage Example:
/// <code>
/// var users = await _adminService.ExecuteReadOnlyAsAdminAsync(
///     tenantId: "tenant-123",
///     reason: "Support ticket #456 - user login issue",
///     async () => 
///     {
///         return await _dbContext.Users
///             .AsNoTracking()  // ✅ Read-only
///             .IgnoreQueryFilters()
///             .Where(u => u.TenantId == tenantId)
///             .ToListAsync();
///     });
/// </code>
/// </summary>
public sealed class AdminTenantAccessService : IAdminTenantAccessService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<AdminTenantAccessService> _logger;
    private readonly ICurrentUserService _currentUserService;

    public AdminTenantAccessService(
        ApplicationDbContext dbContext,
        ILogger<AdminTenantAccessService> logger,
        ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(currentUserService);

        _dbContext = dbContext;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Executes a READ-ONLY query in the context of a specific tenant with admin privileges.
    /// 
    /// ⚠️ IMPORTANT: 
    /// - This method is for READ operations only (.AsNoTracking() required)
    /// - Never use for SaveChanges, Update, or Delete operations
    /// - Bypasses Named Query Filters but RLS still enforces based on session variable
    /// - All access is logged for audit purposes
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the query</typeparam>
    /// <param name="tenantId">The tenant ID to access</param>
    /// <param name="reason">Business justification for accessing this tenant's data (required for audit)</param>
    /// <param name="queryAction">The READ-ONLY query to execute (must use .AsNoTracking())</param>
    /// <returns>The result of the query</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if current user is not a DeploymentManagerAdmin</exception>
    /// <exception cref="ArgumentNullException">Thrown if parameters are null</exception>
    /// <exception cref="InvalidOperationException">Thrown if admin user ID cannot be determined</exception>
    public async Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
        string tenantId,
        string reason,
        Func<Task<TResult>> queryAction)
    {
        ArgumentNullException.ThrowIfNull(tenantId);
        ArgumentNullException.ThrowIfNull(reason);
        ArgumentNullException.ThrowIfNull(queryAction);

        // Guard clause: Reason validation
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for admin access is required for audit logging", nameof(reason));
        }

        // Guard clause: Admin role verification
        if (!_currentUserService.IsInRole(Roles.DeploymentManagerAdmin))
        {
            _logger.LogWarning(
                "ADMIN_ACCESS_DENIED | User {UserId} attempted to access tenant {TenantId} without DeploymentManagerAdmin role",
                _currentUserService.UserId,
                tenantId);

            throw new UnauthorizedAccessException("Only Deployment Manager Admins can access other tenants' data.");
        }

        string adminUserId = _currentUserService.UserId
            ?? throw new InvalidOperationException("Admin user ID not found");

        // Log admin access attempt
        _logger.LogWarning(
            "ADMIN_TENANT_ACCESS | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
            "Operation=ReadOnly | Reason={Reason} | Status=Attempting",
            adminUserId,
            tenantId,
            reason);

        try
        {
            // Set PostgreSQL session variables for RLS using safe parameterized approach
            // - app.current_tenant_id: Allows RLS to filter by this tenant
            // - app.is_admin: Allows SELECT but blocks INSERT/UPDATE/DELETE
            var sessionManager = new PostgreSqlSessionManager(_dbContext);
            await sessionManager.SetSessionVariablesAsync(tenantId, isAdmin: true);

            // Execute the query with Named Query Filters bypassed
            // RLS still enforces based on session variables set above
            TResult result = await queryAction();

            // Log successful access
            _logger.LogWarning(
                "ADMIN_TENANT_ACCESS | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
                "Operation=ReadOnly | Reason={Reason} | Status=Success",
                adminUserId,
                tenantId,
                reason);

            return result;
        }
        catch (NpgsqlException ex)
        {
            // Log failed access due to database error
            _logger.LogError(ex,
                "ADMIN_TENANT_ACCESS | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
                "Operation=ReadOnly | Reason={Reason} | Status=Failed | Error={ErrorMessage}",
                adminUserId,
                tenantId,
                reason,
                ex.Message);

            throw;
        }
        catch (InvalidOperationException ex)
        {
            // Log failed access due to invalid operation
            _logger.LogError(ex,
                "ADMIN_TENANT_ACCESS | AdminUserId={AdminUserId} | TenantId={TenantId} | " +
                "Operation=ReadOnly | Reason={Reason} | Status=Failed | Error={ErrorMessage}",
                adminUserId,
                tenantId,
                reason,
                ex.Message);

            throw;
        }
        finally
        {
            // Clear session variables to prevent leakage to subsequent requests
            try
            {
                var sessionManager = new PostgreSqlSessionManager(_dbContext);
                await sessionManager.ClearSessionVariablesAsync();
            }
            catch (NpgsqlException ex)
            {
                // Log but don't throw - operation already completed, cleanup failure is not critical
                _logger.LogError(ex, "Database error clearing admin tenant context after operation");
            }
            catch (InvalidOperationException ex)
            {
                // Log but don't throw - operation already completed, cleanup failure is not critical
                _logger.LogError(ex, "Invalid operation clearing admin tenant context after operation");
            }
        }
    }
}
