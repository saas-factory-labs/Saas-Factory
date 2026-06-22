using System.Linq;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>A real-time notification that an admin performed a sensitive data access.</summary>
public sealed record AdminAccessAlert(
    string AdminUserId,
    string? AdminEmail,
    string AppSlug,
    string? TenantId,
    string Reason,
    bool IsAutomatedBypass,
    DateTimeOffset OccurredAtUtc);

/// <summary>
/// Raises immediate alerts to the security team when sensitive/cross-tenant data is accessed,
/// per the real-time alerting mitigation.
/// </summary>
public interface IAdminAlertingService
{
    Task RaiseAccessAlertAsync(AdminAccessAlert alert, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default implementation: emits a structured high-severity log event standing in for the
/// email/webhook notification (a real transport can replace this without touching callers).
/// </summary>
public sealed class AdminAlertingService : IAdminAlertingService
{
    private readonly ILogger<AdminAlertingService> _logger;

    public AdminAlertingService(ILogger<AdminAlertingService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public Task RaiseAccessAlertAsync(AdminAccessAlert alert, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(alert);

        string adminUserId = SanitizeForLog(alert.AdminUserId);
        string adminEmail = SanitizeForLog(alert.AdminEmail);
        string appSlug = SanitizeForLog(alert.AppSlug);
        string tenantId = SanitizeForLog(alert.TenantId);
        string reason = SanitizeForLog(alert.Reason);

        _logger.LogWarning(
            "ADMIN_ACCESS_ALERT | admin={AdminUserId} ({AdminEmail}) app={AppSlug} tenant={TenantId} " +
            "bypass={IsAutomatedBypass} reason={Reason} at={OccurredAtUtc:O}",
            adminUserId,
            adminEmail,
            appSlug,
            tenantId,
            alert.IsAutomatedBypass,
            reason,
            alert.OccurredAtUtc);

        return Task.CompletedTask;
    }

    private static string SanitizeForLog(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return new string(value.Where(c => !char.IsControl(c)).ToArray());
    }
}
