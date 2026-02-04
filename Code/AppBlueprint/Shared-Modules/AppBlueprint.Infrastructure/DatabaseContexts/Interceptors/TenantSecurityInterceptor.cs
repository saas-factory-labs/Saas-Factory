using AppBlueprint.Infrastructure.Services;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Interceptors;

/// <summary>
/// Tenant Security Interceptor (Defense-in-Depth Layer 1b).
/// Validates that all modified entities belong to the current tenant.
/// This prevents Cross-Tenant Data Modification (OWASP Top 10) even if a developer 
/// accidentally retrieves an entity from another tenant and attempts to modify it.
/// </summary>
public sealed class TenantSecurityInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public TenantSecurityInterceptor(ITenantContextAccessor tenantContextAccessor)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ValidateTenantScope(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ValidateTenantScope(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void ValidateTenantScope(DbContext? context)
    {
        if (context is null) return;

        // Skip validation if no tenant context is available (e.g. background jobs, migrations)
        // or if explicitly disabled (e.g. admin override)
        var currentTenantId = _tenantContextAccessor.TenantId;
        if (string.IsNullOrEmpty(currentTenantId))
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ITenantScoped>())
        {
            // Only validate modifications and deletions
            // Additions are usually safe (assigned current tenant), but could be validated too if needed
            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                var entityTenantId = entry.Entity.TenantId;

                // CRITICAL SECURITY CHECK
                if (!string.Equals(entityTenantId, currentTenantId, StringComparison.Ordinal))
                {
                    throw new SecurityException(
                        $"Cross-tenant modification detected! " +
                        $"Attempted to {entry.State} entity {entry.Entity.GetType().Name} " +
                        $"belonging to tenant '{entityTenantId}' " +
                        $"while in context of tenant '{currentTenantId}'. " +
                        $"Operation aborted."
                    );
                }
            }
        }
    }
}
