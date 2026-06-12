using AppBlueprint.SharedKernel;

namespace AppBlueprint.AdminPortalKernel.Domain;

/// <summary>
/// Immutable audit trail of every admin portal action, stored in DeploymentManager's own
/// database (table dm_admin_audit) - never in the target app databases. Auditing is a
/// security control: when an audit write fails, the admin operation fails with it.
/// </summary>
public sealed class AdminAuditEntryEntity
{
    public string Id { get; set; } = PrefixedUlid.Generate("audit");

    /// <summary>Slug of the admin portal module (app) the action was performed in.</summary>
    public string AppSlug { get; set; } = string.Empty;

    /// <summary>Stable id ("sub" claim) of the admin who performed the action.</summary>
    public string AdminUserId { get; set; } = string.Empty;

    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>Machine-readable action name, e.g. "user.deactivate" or "users.search".</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Kind of the affected resource, e.g. "User" or "Tenant".</summary>
    public string? TargetType { get; set; }

    /// <summary>Id of the affected resource in the target app database.</summary>
    public string? TargetId { get; set; }

    /// <summary>Tenant scope of the action inside the target app, when applicable.</summary>
    public string? TenantId { get; set; }

    /// <summary>Mandatory human-readable justification entered by the admin.</summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>Optional structured details (JSON).</summary>
    public string? Details { get; set; }

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
