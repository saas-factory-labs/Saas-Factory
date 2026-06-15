using System.Text.Json;

namespace DeploymentManager.Web.Services.Impersonation;

/// <summary>What the UI / API caller must supply to start an impersonation session.</summary>
public sealed record StartImpersonationRequest
{
    /// <summary>Slug of the admin portal module (app) the tenant belongs to. Required for audit scoping.</summary>
    public required string AppSlug { get; init; }

    /// <summary>Tenant being impersonated, in the target app's database.</summary>
    public required string TenantId { get; init; }

    /// <summary>Display name of the tenant (banner copy only).</summary>
    public string TenantName { get; init; } = string.Empty;

    /// <summary>Logto subject ("sub") of the user to act as inside the tenant.</summary>
    public required string TargetUserId { get; init; }

    /// <summary>Mandatory human-readable justification - persisted verbatim to the audit log.</summary>
    public required string Reason { get; init; }
}

/// <summary>Raw result of minting an impersonation token at Logto.</summary>
public sealed record ImpersonationTokenResult(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAtUtc);

/// <summary>
/// An active impersonation session held by the control plane. The token is Logto-signed and scoped
/// read-only; it is consumed by the *target app*, never by the control plane's own (cookie) session.
/// </summary>
public sealed record ImpersonationSession
{
    public required string AppSlug { get; init; }
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
    public required string TargetUserId { get; init; }

    /// <summary>"sub" of the admin who started the session (recorded as the impersonator/actor).</summary>
    public required string ImpersonatorAdminId { get; init; }

    public required string Reason { get; init; }

    /// <summary>
    /// Logto-signed access token for the target subject. The impersonating admin is recorded in the
    /// subject-token context (surfaced via a Logto custom-JWT-claims script, or as the standard
    /// <c>act</c> claim if an actor_token is supplied to the exchange). "Read-only" is the requested
    /// scope - actual enforcement is the target app's resource server, not the control plane.
    /// </summary>
    public required string AccessToken { get; init; }

    public required DateTime StartedAtUtc { get; init; }
    public required DateTime ExpiresAtUtc { get; init; }

    /// <summary>Id of the audit entry written when the session started.</summary>
    public required string AuditEntryId { get; init; }

    /// <summary>Read-only act-as is the only mode this control plane mints.</summary>
    public bool IsReadOnly { get; init; } = true;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
}

/// <summary>
/// Single source of truth for how an impersonation event is recorded, so the REST controller and the
/// in-circuit Blazor service write identical audit semantics despite resolving identity differently.
/// </summary>
public static class ImpersonationAudit
{
    public const string StartAction = "tenant.impersonate.start";
    public const string ExitAction = "tenant.impersonate.exit";
    public const string TargetType = "User";

    /// <summary>Structured (JSON) audit detail describing the impersonation grant.</summary>
    public static string BuildDetails(string targetUserId, DateTime expiresAtUtc, bool readOnly) =>
        JsonSerializer.Serialize(new
        {
            targetUserId,
            expiresAtUtc,
            scope = readOnly ? "read-only" : "read-write",
            via = "logto-token-exchange"
        });
}
