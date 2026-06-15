namespace DeploymentManager.Web.Services.Impersonation;

/// <summary>
/// Configuration for Logto-native impersonation (RFC 8693 token exchange). The control plane never
/// signs tokens itself - target apps reject any non-Logto issuer - so it authenticates to Logto's
/// Management API with its own M2M credentials, creates a one-time subject token for the target user
/// and exchanges it for a Logto-signed, read-only access token carrying the <c>act</c> (actor) claim.
/// </summary>
public sealed class ImpersonationOptions
{
    public const string SectionName = "Impersonation";

    /// <summary>Client id of the Logto machine-to-machine app granted Management API access.</summary>
    public string? ManagementClientId { get; set; }

    /// <summary>Client secret of that M2M app. Supply via Key Vault / env, never source control.</summary>
    public string? ManagementClientSecret { get; set; }

    /// <summary>
    /// Management API resource indicator. Defaults to <c>{LogtoEndpoint}/api</c> when not set
    /// (Logto cloud uses e.g. https://{tenant}.logto.app/api).
    /// </summary>
    public string? ManagementApiResource { get; set; }

    /// <summary>API resource the impersonation token is minted for (the target app's audience).</summary>
    public string? TargetApiResource { get; set; }

    /// <summary>Scope requested for the exchanged token. Read-only act-as by design.</summary>
    public string ReadOnlyScope { get; set; } = "read";

    /// <summary>Lifetime of an impersonation session in minutes. Clamped to [1, 30].</summary>
    public int TokenLifetimeMinutes { get; set; } = 20;

    public int EffectiveLifetimeMinutes => Math.Clamp(TokenLifetimeMinutes, 1, 30);
}
