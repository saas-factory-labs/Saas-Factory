namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Security hardening configuration for the admin portal, bound from the
/// <c>AdminPortal:Security</c> configuration section. All cross-tenant and administrative
/// data access is gated by the values here via <see cref="IAdminAccessGuard"/>.
/// Railway env var format: <c>AdminPortal__Security__MaxTenantsPerHour</c> etc.
/// </summary>
public sealed class AdminPortalSecurityOptions
{
    public const string SectionName = "AdminPortal:Security";

    /// <summary>Maximum number of distinct tenants a single admin may extract per rolling hour.</summary>
    public int MaxTenantsPerHour { get; set; } = 10;

    /// <summary>
    /// When true the guard requires the signed-in admin to carry an MFA claim issued by the
    /// auth provider (Logto). MFA enrolment/verification is owned by Logto - the portal only
    /// enforces that it happened.
    /// </summary>
    public bool RequireMfaClaim { get; set; } = true;

    /// <summary>Claim type that carries the authentication methods (Logto emits <c>amr</c>).</summary>
    public string MfaClaimType { get; set; } = "amr";

    /// <summary>Value within <see cref="MfaClaimType"/> indicating a second factor was used (e.g. "mfa", "otp").</summary>
    public string MfaClaimValue { get; set; } = "mfa";

    /// <summary>
    /// Stable user ids ("sub" claims) of primary super-admins (system owners) who get the
    /// seamless, audited bypass of the manual reason/ticket prompt. Empty means no bypass.
    /// </summary>
    public IList<string> SuperAdminSubjects { get; } = new List<string>();

    /// <summary>External SIEM sink configuration (Splunk/DataDog target).</summary>
    public SiemOptions Siem { get; } = new();

    /// <summary>Real-time security alerting configuration.</summary>
    public AlertingOptions Alerting { get; } = new();
}

/// <summary>Where structured admin-access audit payloads are streamed for tamper-evident retention.</summary>
public sealed class SiemOptions
{
    /// <summary>HTTPS endpoint of the external log collector. Null/empty keeps the sink local (log-only).</summary>
    public string? Endpoint { get; set; }

    /// <summary>Bearer/API key for the collector, sent as an Authorization header when present.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Target system shape for the payload envelope: <c>Splunk</c>, <c>DataDog</c> or <c>Generic</c>.</summary>
    public string Target { get; set; } = "Generic";
}

/// <summary>Where real-time admin-access alerts are delivered.</summary>
public sealed class AlertingOptions
{
    /// <summary>Webhook (Slack/Teams/PagerDuty) the alert is POSTed to. Null/empty keeps alerts local (log-only).</summary>
    public string? WebhookUrl { get; set; }

    /// <summary>Mailbox the security team monitors for sensitive-access notifications.</summary>
    public string? SecurityTeamEmail { get; set; }
}
