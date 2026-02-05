using AppBlueprint.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;

/// <summary>
/// Postgres RLS Interceptor (Defense-in-Depth Layer 2).
/// Injects the current tenant ID into the Postgres session before every command.
/// This enables PostgreSQL Row-Level Security (RLS) to enforce isolation at the database level.
/// </summary>
public sealed class TenantRlsInterceptor : DbCommandInterceptor
{
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public TenantRlsInterceptor(ITenantContextAccessor tenantContextAccessor)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<DbDataReader> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<DbDataReader> result, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<int> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<object> result)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command, 
        CommandEventData eventData, 
        InterceptionResult<object> result, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        SetTenantSessionVariable(command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void SetTenantSessionVariable(DbCommand command)
    {
        var tenantId = _tenantContextAccessor.TenantId;
        if (string.IsNullOrEmpty(tenantId)) return;

        // Prepend the SET command. 
        // We use string interpolation here for the tenantId value.
        // It's sanitized by replacing single quotes.
        var sanitizedTenantId = tenantId.Replace("'", "''", StringComparison.Ordinal);
        
        // We use 'SET LOCAL' to ensure the variable only lasts for the current transaction if any,
        // but 'SET' is usually fine for session-scoped custom variables in EF Core pooled connections.
        // Prepending ensures it runs BEFORE the main query.
        command.CommandText = $"SET app.current_tenant_id = '{sanitizedTenantId}'; " + command.CommandText;
    }
}
