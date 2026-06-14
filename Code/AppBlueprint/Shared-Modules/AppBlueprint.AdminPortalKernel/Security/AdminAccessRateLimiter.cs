using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Caps how many distinct tenants a single admin may extract per rolling hour, the
/// anti-bulk-extraction control from the security reference. Re-accessing a tenant already
/// touched inside the window does not consume additional budget.
/// </summary>
public interface IAdminAccessRateLimiter
{
    /// <summary>
    /// Registers an access to <paramref name="tenantScope"/> by <paramref name="adminUserId"/>.
    /// Returns false when admitting a *new* distinct tenant would exceed the configured cap.
    /// </summary>
    bool TryRegisterTenantAccess(string adminUserId, string tenantScope);
}

/// <summary>
/// In-memory sliding-window limiter. State is per-process and resets on restart; swap for a
/// distributed store when running multi-instance.
/// </summary>
public sealed class AdminAccessRateLimiter : IAdminAccessRateLimiter
{
    private static readonly TimeSpan Window = TimeSpan.FromHours(1);
    private readonly ConcurrentDictionary<string, List<TenantAccess>> _accesses = new(StringComparer.Ordinal);
    private readonly TimeProvider _timeProvider;
    private readonly int _maxTenantsPerHour;

    public AdminAccessRateLimiter(IOptions<AdminPortalSecurityOptions> options, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
        _maxTenantsPerHour = options.Value.MaxTenantsPerHour;
    }

    public bool TryRegisterTenantAccess(string adminUserId, string tenantScope)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(adminUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantScope);

        DateTimeOffset now = _timeProvider.GetUtcNow();
        DateTimeOffset cutoff = now - Window;
        List<TenantAccess> log = _accesses.GetOrAdd(adminUserId, static _ => new List<TenantAccess>());

        lock (log)
        {
            log.RemoveAll(a => a.At < cutoff);

            bool alreadyCounted = log.Exists(a => string.Equals(a.Scope, tenantScope, StringComparison.Ordinal));
            if (!alreadyCounted)
            {
                int distinct = log.Select(a => a.Scope).Distinct(StringComparer.Ordinal).Count();
                if (distinct >= _maxTenantsPerHour)
                {
                    return false;
                }
            }

            log.Add(new TenantAccess(tenantScope, now));
            return true;
        }
    }

    private readonly record struct TenantAccess(string Scope, DateTimeOffset At);
}
