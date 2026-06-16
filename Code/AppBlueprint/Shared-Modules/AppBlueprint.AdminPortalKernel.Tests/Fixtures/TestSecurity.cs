using AppBlueprint.AdminPortalKernel.Security;
using AppBlueprint.AdminPortalKernel.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests.Fixtures;

/// <summary>
/// Builds a real <see cref="AdminAccessGuard"/> for tests that exercise the data path rather than
/// the security pipeline itself: MFA disabled, generous rate limit, no super-admin, and a no-op
/// SIEM sink. Pass a custom sink/alerting/options to assert on the pipeline.
/// </summary>
internal static class TestSecurity
{
    public static AdminAccessGuard PermissiveGuard(
        IAdminPortalUserContext userContext,
        IExternalAuditLogSink? siem = null,
        IAdminAlertingService? alerting = null,
        AdminPortalSecurityOptions? options = null)
    {
        AdminPortalSecurityOptions effectiveOptions = options is null
            ? new AdminPortalSecurityOptions { RequireMfaClaim = false }
            : options;
        IExternalAuditLogSink sink = siem is null ? Substitute.For<IExternalAuditLogSink>() : siem;
        IAdminAlertingService alertingService = alerting is null
            ? new AdminAlertingService(NullLogger<AdminAlertingService>.Instance)
            : alerting;
        TimeProvider time = TimeProvider.System;

        return new AdminAccessGuard(
            userContext,
            new AdminAccountLockoutService(time),
            new AdminAccessRateLimiter(Options.Create(effectiveOptions), time),
            new DeviceFingerprintService(),
            new AdminNonceService(time),
            new TicketValidationService(),
            alertingService,
            sink,
            Options.Create(effectiveOptions),
            time,
            NullLogger<AdminAccessGuard>.Instance);
    }
}
