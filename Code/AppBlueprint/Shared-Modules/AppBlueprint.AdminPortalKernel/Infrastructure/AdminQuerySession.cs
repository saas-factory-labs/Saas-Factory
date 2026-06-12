using AppBlueprint.AdminPortalKernel.Services;
using AppBlueprint.Infrastructure.Persistence.Database;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Gatekeeper for every query against a target app's database, mirroring the
/// AdminTenantAccessService pattern: verified DeploymentManagerAdmin role, a mandatory
/// human-readable reason, RLS session variables (app.is_admin / app.current_tenant_id)
/// set for the duration of the call and cleared afterwards, and a structured access log.
/// </summary>
public sealed class AdminQuerySession
{
    private readonly IAdminPortalDbContextFactory _contextFactory;
    private readonly IAdminPortalUserContext _userContext;
    private readonly ILogger<AdminQuerySession> _logger;

    public AdminQuerySession(
        IAdminPortalDbContextFactory contextFactory,
        IAdminPortalUserContext userContext,
        ILogger<AdminQuerySession> logger)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(logger);

        _contextFactory = contextFactory;
        _userContext = userContext;
        _logger = logger;
    }

    /// <summary>
    /// Runs a read-only query against the module's app database. When
    /// <paramref name="tenantId"/> is provided the RLS tenant variable is scoped to it;
    /// otherwise the query runs cross-tenant with the admin flag only.
    /// </summary>
    public async Task<TResult> ExecuteReadAsync<TResult>(
        string slug,
        string reason,
        Func<AdminPortalAppDbContext, Task<TResult>> query,
        string? tenantId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentNullException.ThrowIfNull(query);
        await EnsureDeploymentManagerAdminAsync();

        await LogAccessIntentAsync("READ", slug, reason, tenantId);

        AdminPortalAppDbContext context = _contextFactory.CreateForModule(slug);
        await using (context)
        {
            var sessionManager = new PostgreSqlSessionManager(context);
            try
            {
                if (tenantId is not null)
                {
                    await sessionManager.SetSessionVariablesAsync(tenantId, isAdmin: true);
                }
                else
                {
                    await sessionManager.SetAdminFlagAsync(true);
                }

                return await query(context);
            }
            finally
            {
                await sessionManager.ClearSessionVariablesAsync();
            }
        }
    }

    /// <summary>
    /// Runs a targeted write (ExecuteUpdate-style) against the module's app database.
    /// Returns the number of affected rows. Callers are responsible for writing the
    /// matching audit entry via IAdminAuditWriter.
    /// </summary>
    public async Task<int> ExecuteWriteAsync(
        string slug,
        string reason,
        Func<AdminPortalAppDbContext, Task<int>> write)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentNullException.ThrowIfNull(write);
        await EnsureDeploymentManagerAdminAsync();

        await LogAccessIntentAsync("WRITE", slug, reason, tenantId: null);

        AdminPortalAppDbContext context = _contextFactory.CreateForModule(slug);
        await using (context)
        {
            var sessionManager = new PostgreSqlSessionManager(context);
            try
            {
                await sessionManager.SetAdminFlagAsync(true);
                return await write(context);
            }
            finally
            {
                await sessionManager.ClearSessionVariablesAsync();
            }
        }
    }

    private async Task EnsureDeploymentManagerAdminAsync()
    {
        if (!await _userContext.IsDeploymentManagerAdminAsync())
        {
            throw new UnauthorizedAccessException(
                "Admin portal access requires the DeploymentManagerAdmin role.");
        }
    }

    private async Task LogAccessIntentAsync(string operation, string slug, string reason, string? tenantId)
    {
        string? userId = await _userContext.GetUserIdAsync();
        string? email = await _userContext.GetEmailAsync();

        _logger.LogInformation(
            "ADMIN_PORTAL_ACCESS: {Operation} app={Slug} by={AdminEmail} ({AdminUserId}) tenant={TenantId} reason={Reason}",
            operation, slug, email, userId, tenantId, reason);
    }
}
