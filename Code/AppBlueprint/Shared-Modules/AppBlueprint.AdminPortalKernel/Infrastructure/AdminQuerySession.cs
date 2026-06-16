using System.Data.Common;
using AppBlueprint.AdminPortalKernel.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Gatekeeper for every query against a target app's database. Authorization and the full
/// security pipeline (role, MFA, rate limit, device fingerprint, nonce, ticket, alerting and
/// SIEM streaming) are delegated to <see cref="IAdminAccessGuard"/>; this type owns the data
/// concerns: RLS session variables and the transaction they live in.
/// <para>
/// The RLS variables (<c>app.is_admin</c> / <c>app.current_tenant_id</c>) are set
/// transaction-locally (<c>set_config(..., is_local =&gt; true</c>)) inside an explicit
/// transaction. This is essential with a pooled NpgsqlDataSource: a transaction pins one
/// physical connection, so the variables are guaranteed to apply to the query that
/// follows, and they are discarded automatically when the transaction ends (no pool
/// leakage, no manual reset).
/// </para>
/// </summary>
public sealed class AdminQuerySession
{
    private readonly IAdminPortalDbContextFactory _contextFactory;
    private readonly IAdminAccessGuard _guard;

    public AdminQuerySession(IAdminPortalDbContextFactory contextFactory, IAdminAccessGuard guard)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);
        ArgumentNullException.ThrowIfNull(guard);

        _contextFactory = contextFactory;
        _guard = guard;
    }

    /// <summary>
    /// Runs a read-only query against the module's app database. When
    /// <paramref name="tenantId"/> is provided the RLS tenant variable is scoped to it;
    /// otherwise the query runs cross-tenant relying on the admin flag (the baseline
    /// SELECT policy is <c>"TenantId" = get_current_tenant() OR is_admin_user()</c>).
    /// <para>
    /// <paramref name="sensitivity"/>, <paramref name="nonce"/> and <paramref name="deviceSignals"/>
    /// drive the security guard: High-sensitivity extractions require a justifying ticket (waived
    /// for the super-admin) and a single-use nonce. Existing low-sensitivity callers can omit them.
    /// </para>
    /// </summary>
    public async Task<TResult> ExecuteReadAsync<TResult>(
        string slug,
        string reason,
        Func<AdminPortalAppDbContext, Task<TResult>> query,
        string? tenantId = null,
        AdminAccessSensitivity sensitivity = AdminAccessSensitivity.Low,
        string? nonce = null,
        DeviceSignals? deviceSignals = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentNullException.ThrowIfNull(query);

        await _guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, slug, reason, tenantId, sensitivity, nonce, deviceSignals));

        AdminPortalAppDbContext context = _contextFactory.CreateForModule(slug);
        await using (context)
        {
            try
            {
                await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                await ApplyAdminSessionAsync(context, tenantId);
                TResult result = await query(context);
                await transaction.CommitAsync();
                return result;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException(BuildDbErrorMessage(slug, ex.Message), ex);
            }
        }
    }

    /// <summary>
    /// Runs a targeted write (ExecuteUpdate-style) against the module's app database.
    /// <paramref name="tenantId"/> is required: the baseline write policy is
    /// <c>USING ("TenantId" = get_current_tenant())</c> and does NOT honor the admin flag,
    /// so the affected rows' tenant must be set for the update to match. Returns the number
    /// of affected rows. Callers write the matching audit entry via IAdminAuditWriter.
    /// </summary>
    public async Task<int> ExecuteWriteAsync(
        string slug,
        string reason,
        string tenantId,
        Func<AdminPortalAppDbContext, Task<int>> write,
        AdminAccessSensitivity sensitivity = AdminAccessSensitivity.Low,
        string? nonce = null,
        DeviceSignals? deviceSignals = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentNullException.ThrowIfNull(write);

        await _guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Write, slug, reason, tenantId, sensitivity, nonce, deviceSignals));

        AdminPortalAppDbContext context = _contextFactory.CreateForModule(slug);
        await using (context)
        {
            try
            {
                await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                await ApplyAdminSessionAsync(context, tenantId);
                int affected = await write(context);
                await transaction.CommitAsync();
                return affected;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException(BuildDbErrorMessage(slug, ex.Message), ex);
            }
        }
    }

    /// <summary>
    /// Sets the RLS session variables transaction-locally on the current connection.
    /// Must be called inside an open transaction.
    /// </summary>
    private static async Task ApplyAdminSessionAsync(AdminPortalAppDbContext context, string? tenantId)
    {
        // is_local => true: scoped to the surrounding transaction.
        await context.Database.ExecuteSqlAsync($"SELECT set_config('app.is_admin', 'true', true)");
        string tenant = tenantId is null ? string.Empty : tenantId;
        await context.Database.ExecuteSqlAsync($"SELECT set_config('app.current_tenant_id', {tenant}, true)");
    }

    private static string BuildDbErrorMessage(string slug, string detail) =>
        $"Could not query the app database for module '{slug}'. Verify that " +
        $"AdminPortal:Modules:{slug}:ConnectionString points at an AppBlueprint database " +
        $"with the baseline schema (Users/Tenants tables). Details: {detail}";
}
