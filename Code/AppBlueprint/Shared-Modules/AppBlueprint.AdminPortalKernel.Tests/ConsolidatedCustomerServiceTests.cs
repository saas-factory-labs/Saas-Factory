using AppBlueprint.AdminPortalKernel.Billing;
using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Services;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class ConsolidatedCustomerServiceTests
{
    private static AdminPortalModuleRegistry TwoModuleRegistry()
    {
        var registry = new AdminPortalModuleRegistry();
        registry.Register(new FixtureAdminModule());      // fixture-app / "Fixture App"
        registry.Register(new SecondFixtureAdminModule()); // second-app / "Second App"
        return registry;
    }

    private static PagedResult<AdminTenantRecord> OneTenant(string id, string name) =>
        new([new AdminTenantRecord { Id = id, Name = name, IsActive = true, Email = "a@example.com", CreatedAt = DateTime.UtcNow }], 1, 1, 100);

    [Test]
    public async Task GetAllAsync_TagsEachTenantWithItsApp_AndCollectsPerAppErrors()
    {
        ITenantAdminService tenants = Substitute.For<ITenantAdminService>();
        tenants.SearchAsync("fixture-app", Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(OneTenant("t1", "Acme"));
        // One app fails its security gate - it must be recorded, not abort the whole consolidation.
        tenants.SearchAsync("second-app", Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(Task.FromException<PagedResult<AdminTenantRecord>>(new UnauthorizedAccessException("MFA required")));

        ISaasBillingProvider billing = Substitute.For<ISaasBillingProvider>();
        billing.IsConfigured.Returns(false);
        billing.GetTenantBillingAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(SaasBillingSnapshot.None);

        var service = new ConsolidatedCustomerService(TwoModuleRegistry(), tenants, billing);

        ConsolidatedCustomersResult result = await service.GetAllAsync();

        result.Customers.Should().ContainSingle();
        result.Customers[0].AppName.Should().Be("Fixture App");
        result.Customers[0].TenantName.Should().Be("Acme");
        result.Customers[0].IsPaying.Should().BeFalse();
        result.BillingConfigured.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Should().Contain("Second App").And.Contain("MFA required");
    }

    [Test]
    public async Task GetAllAsync_AppliesBillingSnapshot_WhenProviderConfigured()
    {
        ITenantAdminService tenants = Substitute.For<ITenantAdminService>();
        tenants.SearchAsync("fixture-app", Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(OneTenant("t1", "Acme"));
        tenants.SearchAsync("second-app", Arg.Any<string?>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new PagedResult<AdminTenantRecord>([], 0, 1, 100));

        ISaasBillingProvider billing = Substitute.For<ISaasBillingProvider>();
        billing.IsConfigured.Returns(true);
        billing.GetTenantBillingAsync("fixture-app", "t1", Arg.Any<CancellationToken>())
            .Returns(new SaasBillingSnapshot(true, 99m));

        var service = new ConsolidatedCustomerService(TwoModuleRegistry(), tenants, billing);

        ConsolidatedCustomersResult result = await service.GetAllAsync();

        result.BillingConfigured.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Customers.Should().ContainSingle();
        result.Customers[0].IsPaying.Should().BeTrue();
        result.Customers[0].MonthlyRecurringRevenue.Should().Be(99m);
    }
}
