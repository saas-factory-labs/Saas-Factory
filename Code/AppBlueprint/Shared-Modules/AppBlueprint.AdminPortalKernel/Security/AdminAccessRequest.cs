namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>Kind of data access being authorized.</summary>
public enum AdminAccessOperation
{
    Read,
    Write,
}

/// <summary>
/// How sensitive an access is. <see cref="High"/> accesses (cross-tenant PII, customer
/// consolidation) require a justifying support ticket and a single-use nonce; <see cref="Low"/>
/// accesses (aggregate dashboard stats, in-app searches) do not.
/// </summary>
public enum AdminAccessSensitivity
{
    Low,
    High,
}

/// <summary>
/// Everything the <see cref="IAdminAccessGuard"/> needs to authorize one administrative data
/// access. Built by <c>AdminQuerySession</c> from the calling page's intent.
/// </summary>
public sealed record AdminAccessRequest(
    AdminAccessOperation Operation,
    string AppSlug,
    string Reason,
    string? TenantId = null,
    AdminAccessSensitivity Sensitivity = AdminAccessSensitivity.Low,
    string? Nonce = null,
    DeviceSignals? DeviceSignals = null);

/// <summary>
/// Result of a successful authorization. <see cref="EffectiveReason"/> is the reason that must be
/// recorded (the caller-supplied reason, or the automated bypass string for super-admins).
/// </summary>
public sealed record AdminAccessDecision(
    string EffectiveReason,
    bool IsAutomatedBypass,
    DeviceFingerprint? Fingerprint);
