using AppBlueprint.AdminPortalKernel.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Security;

/// <summary>
/// Single authorization pipeline for every administrative data access. Composes all of the
/// security-hardening controls in one place so that placing it at the data chokepoint
/// (<c>AdminQuerySession</c>) gates every admin page by construction.
/// </summary>
public interface IAdminAccessGuard
{
    /// <summary>
    /// Authorizes an access or throws. Runs, in order: role → MFA → rate limit → device
    /// fingerprint → nonce → ticket (waived for super-admins) → alert → SIEM stream.
    /// Returns the reason that must be persisted for the access.
    /// </summary>
    Task<AdminAccessDecision> AuthorizeAsync(AdminAccessRequest request, CancellationToken cancellationToken = default);
}

/// <inheritdoc />
public sealed class AdminAccessGuard : IAdminAccessGuard
{
    /// <summary>
    /// Reason recorded for a primary super-admin's seamless access. The UI prompt is bypassed but
    /// the audit trail explicitly marks the access as an automated owner bypass - it never
    /// fabricates a human justification.
    /// </summary>
    public const string AutomatedBypassReason = "Super-Admin Automated Bypass - Direct Access";

    private readonly IAdminPortalUserContext _userContext;
    private readonly IAdminAccountLockoutService _lockout;
    private readonly IAdminAccessRateLimiter _rateLimiter;
    private readonly IDeviceFingerprintService _fingerprint;
    private readonly IAdminNonceService _nonce;
    private readonly ITicketValidationService _ticket;
    private readonly IAdminAlertingService _alerting;
    private readonly IExternalAuditLogSink _siem;
    private readonly AdminPortalSecurityOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AdminAccessGuard> _logger;

    public AdminAccessGuard(
        IAdminPortalUserContext userContext,
        IAdminAccountLockoutService lockout,
        IAdminAccessRateLimiter rateLimiter,
        IDeviceFingerprintService fingerprint,
        IAdminNonceService nonce,
        ITicketValidationService ticket,
        IAdminAlertingService alerting,
        IExternalAuditLogSink siem,
        IOptions<AdminPortalSecurityOptions> options,
        TimeProvider timeProvider,
        ILogger<AdminAccessGuard> logger)
    {
        ArgumentNullException.ThrowIfNull(userContext);
        ArgumentNullException.ThrowIfNull(lockout);
        ArgumentNullException.ThrowIfNull(rateLimiter);
        ArgumentNullException.ThrowIfNull(fingerprint);
        ArgumentNullException.ThrowIfNull(nonce);
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentNullException.ThrowIfNull(alerting);
        ArgumentNullException.ThrowIfNull(siem);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(timeProvider);
        ArgumentNullException.ThrowIfNull(logger);

        _userContext = userContext;
        _lockout = lockout;
        _rateLimiter = rateLimiter;
        _fingerprint = fingerprint;
        _nonce = nonce;
        _ticket = ticket;
        _alerting = alerting;
        _siem = siem;
        _options = options.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<AdminAccessDecision> AuthorizeAsync(AdminAccessRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // 1. Role - the baseline gate, identical to the previous inline check.
        if (!await _userContext.IsDeploymentManagerAdminAsync())
        {
            throw new UnauthorizedAccessException("Admin portal access requires the DeploymentManagerAdmin role.");
        }

        string? rawUserId = await _userContext.GetUserIdAsync();
        string userId = string.IsNullOrWhiteSpace(rawUserId) ? "unknown" : rawUserId;
        string? email = await _userContext.GetEmailAsync();

        // 2. Lockout (defense in depth - a session locked out mid-flight cannot keep extracting).
        if (_lockout.IsLockedOut(userId, out DateTimeOffset lockoutUntil))
        {
            throw new UnauthorizedAccessException($"Admin account is locked out until {lockoutUntil:O}.");
        }

        bool isOwner = await _userContext.IsPrimarySuperAdminAsync();

        // 3. MFA gate - enforced even for the super-admin (owner) per the bypass scope decision.
        if (_options.RequireMfaClaim && !await _userContext.HasCompletedMfaAsync())
        {
            _logger.LogWarning("ADMIN_ACCESS_DENIED | admin={AdminUserId} reason=MFA_NOT_VERIFIED", userId);
            throw new UnauthorizedAccessException("Multi-factor authentication is required for admin access.");
        }

        // 4. Rate limit - enforced even for the owner per the bypass scope decision.
        string tenantScope = string.IsNullOrWhiteSpace(request.TenantId)
            ? $"{request.AppSlug}:*"
            : $"{request.AppSlug}:{request.TenantId}";
        if (!_rateLimiter.TryRegisterTenantAccess(userId, tenantScope))
        {
            _logger.LogWarning("ADMIN_RATE_LIMIT_EXCEEDED | admin={AdminUserId} scope={TenantScope}", userId, tenantScope);
            throw new InvalidOperationException("Admin tenant-access rate limit exceeded. Try again later.");
        }

        // 5. Device fingerprint - computed when signals are available; recorded in the audit stream.
        DeviceFingerprint? fingerprint = request.DeviceSignals is null
            ? null
            : _fingerprint.Compute(request.DeviceSignals);

        // 6. Nonce - single-use per sensitive transaction; required for High-sensitivity access.
        if (!string.IsNullOrWhiteSpace(request.Nonce))
        {
            if (!_nonce.TryConsume(userId, request.Nonce))
            {
                throw new UnauthorizedAccessException("This administrative token (nonce) has already been used.");
            }
        }
        else if (request.Sensitivity == AdminAccessSensitivity.High)
        {
            throw new InvalidOperationException("A single-use nonce is required for this administrative operation.");
        }

        // 7. Justification - super-admin bypasses the manual ticket; everyone else needs a valid ticket
        //    for High-sensitivity extractions.
        bool isAutomatedBypass = false;
        string effectiveReason = request.Reason;
        if (isOwner)
        {
            isAutomatedBypass = true;
            effectiveReason = AutomatedBypassReason;
        }
        else if (request.Sensitivity == AdminAccessSensitivity.High
                 && !await _ticket.ValidateAsync(request.Reason, cancellationToken))
        {
            throw new ArgumentException(
                "A valid support ticket reference (e.g. '#12345') is required in the reason for this access.",
                nameof(request));
        }

        DateTimeOffset now = _timeProvider.GetUtcNow();

        // 8. Real-time alert for sensitive or bypassed access.
        if (isAutomatedBypass || request.Sensitivity == AdminAccessSensitivity.High)
        {
            await _alerting.RaiseAccessAlertAsync(
                new AdminAccessAlert(userId, email, request.AppSlug, request.TenantId, effectiveReason, isAutomatedBypass, now),
                cancellationToken);
        }

        // 9. Stream the complete record to the external SIEM on every authorized access.
        await _siem.EmitAsync(
            new AdminAccessAuditPayload(
                userId, email, request.Operation.ToString(), request.AppSlug, request.TenantId,
                effectiveReason, isAutomatedBypass, fingerprint?.Value, now),
            cancellationToken);

        string safeAppSlug = SanitizeForLog(request.AppSlug);

        _logger.LogInformation(
            "ADMIN_PORTAL_ACCESS: {Operation} app={Slug} by={AdminEmail} ({AdminUserId}) tenant={TenantId} bypass={Bypass} reason={Reason}",
            request.Operation, safeAppSlug, email, userId, request.TenantId, isAutomatedBypass, effectiveReason);

        return new AdminAccessDecision(effectiveReason, isAutomatedBypass, fingerprint);
    }

    private static string SanitizeForLog(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value.Replace("\r", string.Empty).Replace("\n", string.Empty);
    }
}
