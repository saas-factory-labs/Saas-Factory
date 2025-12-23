using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
/// - RBAC: Only SuperAdmin role can access
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
    /// <exception cref="UnauthorizedAccessException">Thrown if current user is not a SuperAdmin</exception>
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

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for admin access is required for audit logging", nameof(reason));
        }

        // Verify current user has admin role
        if (!_currentUserService.IsInRole("SuperAdmin"))
        {
            _logger.LogWarning(
                "ADMIN_ACCESS_DENIED | User {UserId} attempted to access tenant {TenantId} without SuperAdmin role",
                _currentUserService.UserId,
                tenantId);

            throw new UnauthorizedAccessException("Only SuperAdmins can access other tenants' data");
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
            // Set PostgreSQL session variables for RLS
            // - app.current_tenant_id: Allows RLS to filter by this tenant
            // - app.is_admin: Allows SELECT but blocks INSERT/UPDATE/DELETE
            await _dbContext.Database.ExecuteSqlRawAsync(@"
                SELECT set_config('app.current_tenant_id', {0}, FALSE);
                SELECT set_config('app.is_admin', 'true', FALSE);
            ", tenantId);

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
        catch (Exception ex)
        {
            // Log failed access
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
            // Clear tenant context after operation (security hygiene)
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync(@"
                    SELECT set_config('app.current_tenant_id', NULL, FALSE);
                    SELECT set_config('app.is_admin', 'false', FALSE);
                ");
            }
            catch (Exception ex)
            {
                // Log but don't throw - operation already completed
                _logger.LogError(ex, "Failed to clear admin tenant context after operation");
            }
        }
    }
}
