using AppBlueprint.AdminPortalKernel.Security;
using AppBlueprint.AdminPortalKernel.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests.Security;

internal sealed class AdminAccessGuardTests
{
    private const string Slug = "boligportal";

    private static IAdminPortalUserContext User(bool admin = true, bool mfa = true, bool owner = false)
    {
        IAdminPortalUserContext ctx = Substitute.For<IAdminPortalUserContext>();
        ctx.IsDeploymentManagerAdminAsync().Returns(admin);
        ctx.GetUserIdAsync().Returns("admin_1");
        ctx.GetEmailAsync().Returns("owner@example.com");
        ctx.HasCompletedMfaAsync().Returns(mfa);
        ctx.IsPrimarySuperAdminAsync().Returns(owner);
        return ctx;
    }

    private static (AdminAccessGuard Guard, IExternalAuditLogSink Siem, IAdminAlertingService Alerting) BuildGuard(
        IAdminPortalUserContext userContext, AdminPortalSecurityOptions options)
    {
        IExternalAuditLogSink siem = Substitute.For<IExternalAuditLogSink>();
        IAdminAlertingService alerting = Substitute.For<IAdminAlertingService>();
        TimeProvider time = TimeProvider.System;

        var guard = new AdminAccessGuard(
            userContext,
            new AdminAccountLockoutService(time),
            new AdminAccessRateLimiter(Options.Create(options), time),
            new DeviceFingerprintService(),
            new AdminNonceService(time),
            new TicketValidationService(),
            alerting,
            siem,
            Options.Create(options),
            time,
            NullLogger<AdminAccessGuard>.Instance);

        return (guard, siem, alerting);
    }

    private static AdminPortalSecurityOptions Options_(bool requireMfa = true, int maxTenants = 10) =>
        new() { RequireMfaClaim = requireMfa, MaxTenantsPerHour = maxTenants };

    [Test]
    public async Task NonAdmin_IsDenied()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(admin: false), Options_());

        Func<Task> act = () => guard.AuthorizeAsync(
            new AdminAccessRequest(AdminAccessOperation.Read, Slug, "dashboard.stats"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task MfaRequiredButNotCompleted_IsDenied()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(mfa: false), Options_(requireMfa: true));

        Func<Task> act = () => guard.AuthorizeAsync(
            new AdminAccessRequest(AdminAccessOperation.Read, Slug, "dashboard.stats"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Multi-factor*");
    }

    [Test]
    public async Task LowSensitivity_Authorizes_StreamsSiem_DoesNotAlert()
    {
        (AdminAccessGuard guard, IExternalAuditLogSink siem, IAdminAlertingService alerting) =
            BuildGuard(User(), Options_());

        AdminAccessDecision decision = await guard.AuthorizeAsync(
            new AdminAccessRequest(AdminAccessOperation.Read, Slug, "dashboard.stats"));

        decision.IsAutomatedBypass.Should().BeFalse();
        decision.EffectiveReason.Should().Be("dashboard.stats");
        await siem.Received(1).EmitAsync(Arg.Any<AdminAccessAuditPayload>(), Arg.Any<CancellationToken>());
        await alerting.DidNotReceive().RaiseAccessAlertAsync(Arg.Any<AdminAccessAlert>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task HighSensitivity_WithoutNonce_IsRejected()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(), Options_());

        Func<Task> act = () => guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "Support ticket #42", "tenant_1",
            AdminAccessSensitivity.High, Nonce: null));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*nonce*");
    }

    [Test]
    public async Task HighSensitivity_NonOwner_WithoutTicket_IsRejected()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(owner: false), Options_());

        Func<Task> act = () => guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "just having a look", "tenant_1",
            AdminAccessSensitivity.High, Nonce: "nonce-1"));

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*ticket*");
    }

    [Test]
    public async Task HighSensitivity_NonOwner_WithTicket_Authorizes_AndAlerts()
    {
        (AdminAccessGuard guard, IExternalAuditLogSink siem, IAdminAlertingService alerting) =
            BuildGuard(User(owner: false), Options_());

        AdminAccessDecision decision = await guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "Investigating, see ticket #4711", "tenant_1",
            AdminAccessSensitivity.High, Nonce: "nonce-1"));

        decision.IsAutomatedBypass.Should().BeFalse();
        decision.EffectiveReason.Should().Be("Investigating, see ticket #4711");
        await alerting.Received(1).RaiseAccessAlertAsync(Arg.Any<AdminAccessAlert>(), Arg.Any<CancellationToken>());
        await siem.Received(1).EmitAsync(Arg.Any<AdminAccessAuditPayload>(), Arg.Any<CancellationToken>());
    }

    // --- Task 3: super-admin transparent bypass invariants ---

    [Test]
    public async Task Owner_BypassesTicket_ButStreamsLabeledBypassToSiemAndAlerts()
    {
        (AdminAccessGuard guard, IExternalAuditLogSink siem, IAdminAlertingService alerting) =
            BuildGuard(User(owner: true), Options_());

        // No ticket in the reason and a High-sensitivity extraction: a non-owner would be rejected.
        AdminAccessDecision decision = await guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, Reason: "n/a", TenantId: "tenant_1",
            Sensitivity: AdminAccessSensitivity.High, Nonce: "nonce-owner"));

        decision.IsAutomatedBypass.Should().BeTrue();
        decision.EffectiveReason.Should().Be(AdminAccessGuard.AutomatedBypassReason);

        await siem.Received(1).EmitAsync(
            Arg.Is<AdminAccessAuditPayload>(p =>
                p.IsAutomatedBypass && p.Reason == AdminAccessGuard.AutomatedBypassReason),
            Arg.Any<CancellationToken>());
        await alerting.Received(1).RaiseAccessAlertAsync(
            Arg.Is<AdminAccessAlert>(a => a.IsAutomatedBypass), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Owner_IsStillSubjectToMfa()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(owner: true, mfa: false), Options_(requireMfa: true));

        Func<Task> act = () => guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "n/a", "tenant_1", AdminAccessSensitivity.High, Nonce: "n1"));

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Multi-factor*");
    }

    [Test]
    public async Task Owner_IsStillSubjectToRateLimit()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(owner: true), Options_(maxTenants: 1));

        await guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "n/a", "tenant_1", AdminAccessSensitivity.High, Nonce: "n1"));

        Func<Task> second = () => guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "n/a", "tenant_2", AdminAccessSensitivity.High, Nonce: "n2"));

        await second.Should().ThrowAsync<InvalidOperationException>().WithMessage("*rate limit*");
    }

    [Test]
    public async Task ReusedNonce_IsRejected_AcrossCalls()
    {
        (AdminAccessGuard guard, _, _) = BuildGuard(User(owner: false), Options_());

        await guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "ticket #1", "tenant_1", AdminAccessSensitivity.High, Nonce: "dup"));

        Func<Task> replay = () => guard.AuthorizeAsync(new AdminAccessRequest(
            AdminAccessOperation.Read, Slug, "ticket #1", "tenant_1", AdminAccessSensitivity.High, Nonce: "dup"));

        await replay.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*nonce*");
    }
}
