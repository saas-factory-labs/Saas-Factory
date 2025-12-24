namespace AppBlueprint.Application.Services;

/// <summary>
/// Service interface for administrators to access tenant data with audit logging.
/// ⚠️ READ-ONLY: Admins can only VIEW tenant data, never modify or delete.
/// 
/// This service enables DeploymentManager and admin portals to view tenant data
/// for support purposes while maintaining strict security controls.
/// 
/// Security guarantees:
/// - READ-ONLY: Uses .AsNoTracking() to prevent modifications
/// - RBAC: Only DeploymentManagerAdmin role can access
/// - AUDIT: All access logged with reason and timestamp
/// - TEMPORARY: Tenant context cleared after operation
/// - DEFENSE-IN-DEPTH: RLS enforces read-only at database level
/// </summary>
public interface IAdminTenantAccessService
{
    /// <summary>
    /// Executes a READ-ONLY query in the context of a specific tenant with admin privileges.
    /// 
    /// ⚠️ IMPORTANT: 
    /// - This method is for READ operations only (.AsNoTracking() required)
    /// - Never use for SaveChanges, Update, or Delete operations
    /// - All access is logged for audit purposes
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the query</typeparam>
    /// <param name="tenantId">The tenant ID to access</param>
    /// <param name="reason">Business justification for accessing this tenant's data (required for audit)</param>
    /// <param name="queryAction">The READ-ONLY query to execute (must use .AsNoTracking())</param>
    /// <returns>The result of the query</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if current user is not a DeploymentManagerAdmin</exception>
    Task<TResult> ExecuteReadOnlyAsAdminAsync<TResult>(
        string tenantId,
        string reason,
        Func<Task<TResult>> queryAction);
}
