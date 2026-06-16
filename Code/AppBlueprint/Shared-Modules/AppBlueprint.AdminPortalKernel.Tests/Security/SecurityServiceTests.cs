using System.Text.Json;
using AppBlueprint.AdminPortalKernel.Security;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Tests.Security;

internal sealed class SecurityServiceTests
{
    private static readonly DateTimeOffset Start = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    // --- Device fingerprint ---

    [Test]
    public async Task Fingerprint_IsStableForSameSignals_AndChangesWhenAnySignalChanges()
    {
        var service = new DeviceFingerprintService();
        var signals = new DeviceSignals("UA/1.0", "203.0.113.5", "en-US", "\"Chromium\";v=\"120\"");

        DeviceFingerprint first = service.Compute(signals);
        DeviceFingerprint same = service.Compute(signals with { });
        DeviceFingerprint differentIp = service.Compute(signals with { IpAddress = "203.0.113.6" });

        first.Value.Should().Be(same.Value);
        first.Value.Should().NotBe(differentIp.Value);
        await Task.CompletedTask;
    }

    // --- Nonce ---

    [Test]
    public async Task Nonce_IsSingleUse_AndReplayIsRejected()
    {
        var service = new AdminNonceService(new FakeTimeProvider(Start));
        string nonce = service.GenerateNonce();

        service.TryConsume("admin_1", nonce).Should().BeTrue();
        service.TryConsume("admin_1", nonce).Should().BeFalse();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Nonce_IsScopedPerAdmin()
    {
        var service = new AdminNonceService(new FakeTimeProvider(Start));
        string nonce = service.GenerateNonce();

        service.TryConsume("admin_1", nonce).Should().BeTrue();
        service.TryConsume("admin_2", nonce).Should().BeTrue();
        await Task.CompletedTask;
    }

    // --- Rate limiter ---

    [Test]
    public async Task RateLimiter_AllowsUpToCap_ThenRejectsNewTenants_ButPermitsReaccess()
    {
        var options = Options.Create(new AdminPortalSecurityOptions { MaxTenantsPerHour = 2 });
        var limiter = new AdminAccessRateLimiter(options, new FakeTimeProvider(Start));

        limiter.TryRegisterTenantAccess("admin_1", "app:t1").Should().BeTrue();
        limiter.TryRegisterTenantAccess("admin_1", "app:t2").Should().BeTrue();
        limiter.TryRegisterTenantAccess("admin_1", "app:t3").Should().BeFalse();      // 3rd distinct tenant blocked
        limiter.TryRegisterTenantAccess("admin_1", "app:t1").Should().BeTrue();        // re-access already-seen tenant ok
        await Task.CompletedTask;
    }

    [Test]
    public async Task RateLimiter_WindowResets_AfterAnHour()
    {
        var time = new FakeTimeProvider(Start);
        var options = Options.Create(new AdminPortalSecurityOptions { MaxTenantsPerHour = 1 });
        var limiter = new AdminAccessRateLimiter(options, time);

        limiter.TryRegisterTenantAccess("admin_1", "app:t1").Should().BeTrue();
        limiter.TryRegisterTenantAccess("admin_1", "app:t2").Should().BeFalse();

        time.Advance(TimeSpan.FromHours(1) + TimeSpan.FromMinutes(1));

        limiter.TryRegisterTenantAccess("admin_1", "app:t2").Should().BeTrue();
        await Task.CompletedTask;
    }

    // --- Lockout ---

    [Test]
    public async Task Lockout_EscalatesProgressively_AndResetClears()
    {
        var time = new FakeTimeProvider(Start);
        var service = new AdminAccountLockoutService(time);

        for (int i = 0; i < 4; i++)
        {
            service.RecordFailedAttempt("owner@example.com");
        }

        service.IsLockedOut("owner@example.com", out _).Should().BeFalse();

        service.RecordFailedAttempt("owner@example.com"); // 5th
        service.IsLockedOut("owner@example.com", out DateTimeOffset after5).Should().BeTrue();
        after5.Should().Be(Start.AddMinutes(15));

        for (int i = 0; i < 5; i++)
        {
            service.RecordFailedAttempt("owner@example.com"); // 10th
        }

        service.IsLockedOut("owner@example.com", out DateTimeOffset after10).Should().BeTrue();
        after10.Should().Be(Start.AddHours(1));

        for (int i = 0; i < 5; i++)
        {
            service.RecordFailedAttempt("owner@example.com"); // 15th
        }

        service.IsLockedOut("owner@example.com", out DateTimeOffset after15).Should().BeTrue();
        after15.Should().Be(Start.AddHours(24));

        service.Reset("owner@example.com");
        service.IsLockedOut("owner@example.com", out _).Should().BeFalse();
        await Task.CompletedTask;
    }

    // --- Ticket validation ---

    [Test]
    public async Task Ticket_RequiresTicketReference()
    {
        var service = new TicketValidationService();

        (await service.ValidateAsync("Support ticket #12345")).Should().BeTrue();
        (await service.ValidateAsync("just looking around")).Should().BeFalse();
        (await service.ValidateAsync("   ")).Should().BeFalse();
    }

    // --- SIEM envelope shaping ---

    [Test]
    public async Task SiemEnvelope_ShapesPerTarget_AndAlwaysCarriesReasonAndBypassFlag()
    {
        var payload = new AdminAccessAuditPayload(
            "admin_1", "owner@example.com", "Read", "boligportal", "tenant_9",
            AdminAccessGuard.AutomatedBypassReason, IsAutomatedBypass: true, "fp-hash", Start);

        string splunk = JsonSerializer.Serialize(ExternalAuditLogSink.BuildEnvelope(payload, "Splunk"));
        string datadog = JsonSerializer.Serialize(ExternalAuditLogSink.BuildEnvelope(payload, "DataDog"));
        string generic = JsonSerializer.Serialize(ExternalAuditLogSink.BuildEnvelope(payload, "anything-else"));

        splunk.Should().Contain("sourcetype").And.Contain("admin_access");
        datadog.Should().Contain("ddsource").And.Contain("app:boligportal");
        generic.Should().Contain("admin_access");
        foreach (string json in new[] { splunk, datadog, generic })
        {
            json.Should().Contain(AdminAccessGuard.AutomatedBypassReason);
            json.Should().Contain("IsAutomatedBypass");
        }

        await Task.CompletedTask;
    }
}
