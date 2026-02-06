using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;

/// <summary>
/// Database connection interceptor that sets the PostgreSQL RLS session variable 
/// for tenant isolation (defense-in-depth Layer 2).
/// 
/// This interceptor executes SET LOCAL app.current_tenant_id = 'tenant-123' on every 
/// database connection, enabling PostgreSQL Row-Level Security policies to enforce 
/// tenant isolation at the database level.
/// </summary>
public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<TenantConnectionInterceptor> _logger;

    public TenantConnectionInterceptor(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<TenantConnectionInterceptor> logger)
    {
        ArgumentNullException.ThrowIfNull(tenantContextAccessor);
        ArgumentNullException.ThrowIfNull(logger);

        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Called after a database connection is opened. Sets the RLS session variable.
    /// </summary>
    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        InterceptionResult baseResult = await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        await SetTenantContextAsync(connection, cancellationToken);
        return baseResult;
    }

    /// <summary>
    /// Called after a database connection is opened synchronously. Sets the RLS session variable.
    /// </summary>
    public override InterceptionResult ConnectionOpening(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        InterceptionResult baseResult = base.ConnectionOpening(connection, eventData, result);
        SetTenantContext(connection);
        return baseResult;
    }

    /// <summary>
    /// Sets the PostgreSQL session variable app.current_tenant_id for RLS enforcement.
    /// </summary>
    private async Task SetTenantContextAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        string? tenantId = _tenantContextAccessor.TenantId;

        // If no tenant context (migrations, background jobs, admin operations), skip setting
        if (tenantId is null)
        {
            _logger.LogDebug("No tenant context available - RLS session variable not set (migrations or system operation)");
            return;
        }

        // Set PostgreSQL session variable for RLS (SET LOCAL is transaction-scoped)
        // Using SET LOCAL ensures the variable is automatically cleared when transaction ends
        if (connection is NpgsqlConnection npgsqlConnection)
        {
            await using NpgsqlCommand command = npgsqlConnection.CreateCommand();
            command.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, FALSE);";
            command.Parameters.AddWithValue("@tenantId", tenantId);

            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken);
                _logger.LogDebug("RLS session variable set: app.current_tenant_id = {TenantId}", tenantId);
            }
            catch (NpgsqlException ex)
            {
                // Log error but don't throw - RLS is defense-in-depth (Layer 1 Named Filters still protect)
                _logger.LogError(ex, "Failed to set RLS session variable for tenant {TenantId}. Named Query Filters (Layer 1) still active.", tenantId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation when setting RLS variable for tenant {TenantId}", tenantId);
            }
        }
    }

    /// <summary>
    /// Synchronous version of SetTenantContextAsync for synchronous connection opening.
    /// </summary>
    private void SetTenantContext(DbConnection connection)
    {
        string? tenantId = _tenantContextAccessor.TenantId;

        if (tenantId is null)
        {
            _logger.LogDebug("No tenant context available - RLS session variable not set (migrations or system operation)");
            return;
        }

        if (connection is NpgsqlConnection npgsqlConnection)
        {
            using NpgsqlCommand command = npgsqlConnection.CreateCommand();
            command.CommandText = "SELECT set_config('app.current_tenant_id', @tenantId, FALSE);";
            command.Parameters.AddWithValue("@tenantId", tenantId);

            try
            {
                command.ExecuteNonQuery();
                _logger.LogDebug("RLS session variable set: app.current_tenant_id = {TenantId}", tenantId);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Failed to set RLS session variable for tenant {TenantId}. Named Query Filters (Layer 1) still active.", tenantId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation when setting RLS variable for tenant {TenantId}", tenantId);
            }
        }
    }
}
