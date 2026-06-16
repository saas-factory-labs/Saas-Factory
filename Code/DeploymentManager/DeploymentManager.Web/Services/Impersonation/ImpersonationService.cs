using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Services;

namespace DeploymentManager.Web.Services.Impersonation;

/// <summary>
/// DISABLED / PARKED: this whole impersonation feature (this service, ImpersonationController,
/// ImpersonationBanner, LogtoImpersonationTokenService) is not wired up - its DI registrations in
/// Program.cs are commented out. It is kept, not deleted, for a possible future product decision.
/// The current requirement (secure, read-only, rate-limited viewing of tenant/user data) is met
/// instead by the kernel's AdminQuerySession/IAdminAccessGuard pipeline and its AdminTenants/AdminUsers
/// pages. See the comments in Program.cs and MainLayout.razor to re-enable.
///
/// Scoped Blazor state container for the admin's active "act-as" session. It mints a Logto-signed
/// read-only token for the target user, audits the action and exposes the session to the UI (banner).
///
/// SECURITY: it deliberately does NOT touch the admin's own control-plane authentication cookie - that
/// session is the audit chain. The minted token belongs to the *target app* (a separate deployment),
/// which is where it is loaded into that app's localStorage; the control plane only holds it for hand-off.
/// </summary>
public sealed class ImpersonationService
{
    private readonly ILogtoImpersonationTokenService _tokenService;
    private readonly IAdminAuditWriter _auditWriter;
    private readonly IAdminPortalUserContext _userContext;
    private readonly ILogger<ImpersonationService> _logger;

    public ImpersonationService(
        ILogtoImpersonationTokenService tokenService,
        IAdminAuditWriter auditWriter,
        IAdminPortalUserContext userContext,
        ILogger<ImpersonationService> logger)
    {
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentNullException.ThrowIfNull(auditWriter);
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(logger);
        _tokenService = tokenService;
        _auditWriter = auditWriter;
        _userContext = userContext;
        _logger = logger;
    }

    /// <summary>The active session, or null when the admin is acting as themselves.</summary>
    public ImpersonationSession? CurrentSession { get; private set; }

    public bool IsActive => CurrentSession is not null && !CurrentSession.IsExpired;

    /// <summary>Raised whenever the session starts or ends so the banner / layout can re-render.</summary>
    public event Action? OnChange;

    /// <summary>
    /// Starts a read-only impersonation session: validates the caller, mints the token, writes the
    /// audit entry (fail-closed) and publishes the session. Throws if the caller is not a
    /// DeploymentManagerAdmin or if anything in the mint/audit pipeline fails.
    /// </summary>
    public async Task<ImpersonationSession> StartAsync(
        StartImpersonationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.AppSlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.TargetUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Reason);

        if (!await _userContext.IsDeploymentManagerAdminAsync())
        {
            throw new UnauthorizedAccessException(
                "Starting an impersonation session requires the DeploymentManagerAdmin role.");
        }

        string adminId = await _userContext.GetUserIdAsync() ?? "unknown";

        ImpersonationTokenResult token = await _tokenService.IssueReadOnlyTokenAsync(
            request.TargetUserId, adminId, cancellationToken);

        // Audit is a security control: if the write fails, the operation fails and no session starts.
        AdminAuditEntryEntity entry = await _auditWriter.WriteAsync(
            appSlug: request.AppSlug,
            action: ImpersonationAudit.StartAction,
            reason: request.Reason,
            targetType: ImpersonationAudit.TargetType,
            targetId: request.TargetUserId,
            tenantId: request.TenantId,
            details: ImpersonationAudit.BuildDetails(request.TargetUserId, token.ExpiresAtUtc, readOnly: true));

        var session = new ImpersonationSession
        {
            AppSlug = request.AppSlug,
            TenantId = request.TenantId,
            TenantName = string.IsNullOrWhiteSpace(request.TenantName) ? request.TenantId : request.TenantName,
            TargetUserId = request.TargetUserId,
            ImpersonatorAdminId = adminId,
            Reason = request.Reason,
            AccessToken = token.AccessToken,
            StartedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = token.ExpiresAtUtc,
            AuditEntryId = entry.Id
        };

        CurrentSession = session;
        _logger.LogInformation(
            "Impersonation started: admin {AdminId} acting as user {TargetUserId} in tenant {TenantId} ({AppSlug}); audit {AuditId}.",
            adminId, request.TargetUserId, request.TenantId, request.AppSlug, entry.Id);

        OnChange?.Invoke();
        return session;
    }

    /// <summary>Ends the active session, restoring the admin's own context, and audits the exit.</summary>
    public async Task ExitAsync(CancellationToken cancellationToken = default)
    {
        ImpersonationSession? session = CurrentSession;
        if (session is null)
        {
            return;
        }

        CurrentSession = null;
        OnChange?.Invoke();

        try
        {
            await _auditWriter.WriteAsync(
                appSlug: session.AppSlug,
                action: ImpersonationAudit.ExitAction,
                reason: session.Reason,
                targetType: ImpersonationAudit.TargetType,
                targetId: session.TargetUserId,
                tenantId: session.TenantId,
                details: ImpersonationAudit.BuildDetails(session.TargetUserId, session.ExpiresAtUtc, readOnly: true));
        }
        catch (Exception ex)
        {
            // Exit must always clear local state; a failed exit-audit is logged but never traps the
            // admin inside an impersonation session.
            _logger.LogError(ex, "Failed to write impersonation-exit audit for tenant {TenantId}.", session.TenantId);
        }
    }
}
