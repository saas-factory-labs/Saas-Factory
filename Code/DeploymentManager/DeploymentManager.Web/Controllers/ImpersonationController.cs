using System.Security.Claims;
using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.Application.Constants;
using DeploymentManager.Web.Services.Impersonation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeploymentManager.Web.Controllers;

/// <summary>
/// Documented REST surface for starting a tenant impersonation ("act-as") session.
///
/// SECURITY (OWASP A01): gated to the global <see cref="Roles.DeploymentManagerAdmin"/> role. The token
/// it returns is minted by Logto (read-only, RFC 8693 token exchange) - the control plane never signs
/// tokens itself - and every call is written to the immutable admin audit log before the token is
/// returned. Identity is resolved from <see cref="HttpContext.User"/> (this is an MVC request, so the
/// circuit-bound <c>IAdminPortalUserContext</c>/<c>IAdminAuditWriter</c> are not available here).
/// </summary>
// DISABLED / PARKED: declared `internal` on purpose. MVC controller discovery only picks up *public*
// types, so as `internal` this endpoint is NOT routed/exposed by MapControllers - the impersonation
// feature is parked pending a product decision (the real requirement, read-only rate-limited viewing,
// is already met by the kernel's AdminTenants/AdminUsers pages). Its dependency
// ILogtoImpersonationTokenService is also unregistered (see the commented DI block in Program.cs).
// TO RE-ENABLE: change `internal` back to `public` and uncomment that DI block.
[ApiController]
[Route("api/admin/impersonate")]
[Authorize(Roles = Roles.DeploymentManagerAdmin)]
internal sealed class ImpersonationController : ControllerBase
{
    private readonly ILogtoImpersonationTokenService _tokenService;
    private readonly IDbContextFactory<AdminPortalAuditDbContext> _auditContextFactory;
    private readonly ILogger<ImpersonationController> _logger;

    public ImpersonationController(
        ILogtoImpersonationTokenService tokenService,
        IDbContextFactory<AdminPortalAuditDbContext> auditContextFactory,
        ILogger<ImpersonationController> logger)
    {
        ArgumentNullException.ThrowIfNull(tokenService);
        ArgumentNullException.ThrowIfNull(auditContextFactory);
        ArgumentNullException.ThrowIfNull(logger);
        _tokenService = tokenService;
        _auditContextFactory = auditContextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Starts a read-only impersonation session for <paramref name="userId"/> within
    /// <paramref name="tenantId"/>. A justification (<see cref="ImpersonationRequestBody.Reason"/>) is
    /// mandatory for audit.
    /// </summary>
    [HttpPost("{tenantId}/{userId}")]
    public async Task<ActionResult<ImpersonationSession>> StartAsync(
        string tenantId,
        string userId,
        [FromBody] ImpersonationRequestBody body,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("tenantId and userId are required.");
        }

        if (body is null || string.IsNullOrWhiteSpace(body.AppSlug) || string.IsNullOrWhiteSpace(body.Reason))
        {
            return BadRequest("appSlug and a reason for impersonation are required.");
        }

        string adminId = User.FindFirstValue("sub")
                         ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? "unknown";
        string adminEmail = User.FindFirstValue("email")
                            ?? User.FindFirstValue(ClaimTypes.Email)
                            ?? "unknown";

        ImpersonationTokenResult token = await _tokenService.IssueReadOnlyTokenAsync(
            userId, adminId, cancellationToken);

        // Fail closed: audit is written (and committed) before the token is handed back.
        var entry = new AdminAuditEntryEntity
        {
            AppSlug = body.AppSlug,
            AdminUserId = adminId,
            AdminEmail = adminEmail,
            Action = ImpersonationAudit.StartAction,
            TargetType = ImpersonationAudit.TargetType,
            TargetId = userId,
            TenantId = tenantId,
            Reason = body.Reason,
            Details = ImpersonationAudit.BuildDetails(userId, token.ExpiresAtUtc, readOnly: true),
            OccurredAtUtc = DateTime.UtcNow
        };

        AdminPortalAuditDbContext context = await _auditContextFactory.CreateDbContextAsync(cancellationToken);
        await using (context)
        {
            context.AuditEntries.Add(entry);
            await context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Impersonation started via API: admin {AdminId} acting as user {TargetUserId} in tenant {TenantId} ({AppSlug}); audit {AuditId}.",
            adminId, userId, tenantId, body.AppSlug, entry.Id);

        var session = new ImpersonationSession
        {
            AppSlug = body.AppSlug,
            TenantId = tenantId,
            TenantName = string.IsNullOrWhiteSpace(body.TenantName) ? tenantId : body.TenantName,
            TargetUserId = userId,
            ImpersonatorAdminId = adminId,
            Reason = body.Reason,
            AccessToken = token.AccessToken,
            StartedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = token.ExpiresAtUtc,
            AuditEntryId = entry.Id
        };

        return Ok(session);
    }

    /// <summary>Body for <see cref="StartAsync"/>: app scope, optional display name and the mandatory reason.</summary>
    public sealed record ImpersonationRequestBody
    {
        public required string AppSlug { get; init; }
        public string TenantName { get; init; } = string.Empty;
        public required string Reason { get; init; }
    }
}
