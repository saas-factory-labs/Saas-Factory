using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests.Integration;

internal sealed class AdminServicesTests
{
    private const string Slug = "fixture-app";

    private sealed class TestAuditContextFactory : IDbContextFactory<AdminPortalAuditDbContext>
    {
        private readonly DbContextOptions<AdminPortalAuditDbContext> _options;

        public TestAuditContextFactory(string connectionString)
        {
            _options = new DbContextOptionsBuilder<AdminPortalAuditDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        public AdminPortalAuditDbContext CreateDbContext() => new(_options);
    }

    private sealed record ServiceHarness(
        UserAdminService Users,
        TenantAdminService Tenants,
        DashboardService Dashboard,
        IAdminAuditReader AuditReader,
        string AppDbConnectionString);

    private static IAdminPortalUserContext CreateAdminUserContext(bool isAdmin = true)
    {
        IAdminPortalUserContext userContext = Substitute.For<IAdminPortalUserContext>();
        userContext.IsDeploymentManagerAdminAsync().Returns(isAdmin);
        userContext.GetUserIdAsync().Returns("admin_user_1");
        userContext.GetEmailAsync().Returns("owner@example.com");
        return userContext;
    }

    private static async Task<ServiceHarness> CreateHarnessAsync(bool isAdmin = true)
    {
        string appDbConnectionString = await PostgresFixture.GetConnectionStringAsync();
        string auditConnectionString = await PostgresFixture.GetAuditConnectionStringAsync();

        var options = new AdminPortalOptions();
        options.Modules[Slug] = new AdminPortalModuleOptions { ConnectionString = appDbConnectionString };
        var factory = new AdminPortalDbContextFactory(Options.Create(options));

        IAdminPortalUserContext userContext = CreateAdminUserContext(isAdmin);
        var session = new AdminQuerySession(factory, userContext, NullLogger<AdminQuerySession>.Instance);

        var auditFactory = new TestAuditContextFactory(auditConnectionString);
        await using (AdminPortalAuditDbContext auditContext = auditFactory.CreateDbContext())
        {
            await auditContext.Database.EnsureCreatedAsync();
        }

        var auditWriter = new AdminAuditWriter(auditFactory, userContext);
        var auditReader = new AdminAuditReader(auditFactory, userContext);

        return new ServiceHarness(
            new UserAdminService(session, auditWriter),
            new TenantAdminService(session),
            new DashboardService(session),
            auditReader,
            appDbConnectionString);
    }

    [Test]
    public async Task SearchAsync_MatchesCaseInsensitive_AcrossNameAndEmail()
    {
        ServiceHarness harness = await CreateHarnessAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string marker = Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(harness.AppDbConnectionString, tenantId, "Search Tenant");
        await PostgresFixture.InsertUserAsync(
            harness.AppDbConnectionString, $"user_a_{marker}", $"alpha.{marker}@example.com", tenantId,
            firstName: $"Zelda{marker}", lastName: "Searchable");
        await PostgresFixture.InsertUserAsync(
            harness.AppDbConnectionString, $"user_b_{marker}", $"beta.{marker}@example.com", tenantId,
            firstName: "Other", lastName: "Person");

        PagedResult<AdminUserRecord> byName = await harness.Users.SearchAsync(
            Slug, new UserSearchRequest { SearchText = $"zelda{marker}" });
        PagedResult<AdminUserRecord> byEmail = await harness.Users.SearchAsync(
            Slug, new UserSearchRequest { SearchText = $"ALPHA.{marker}" });

        byName.TotalCount.Should().Be(1);
        byName.Items.Single().FirstName.Should().Be($"Zelda{marker}");
        byEmail.TotalCount.Should().Be(1);
        byEmail.Items.Single().Email.Should().Be($"alpha.{marker}@example.com");
    }

    [Test]
    public async Task SearchAsync_FiltersByTenant_AndPages()
    {
        ServiceHarness harness = await CreateHarnessAsync();
        string tenantA = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string tenantB = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(harness.AppDbConnectionString, tenantA, "Tenant A");
        await PostgresFixture.InsertTenantAsync(harness.AppDbConnectionString, tenantB, "Tenant B");

        for (int i = 0; i < 3; i++)
        {
            string id = $"user_{Guid.NewGuid().ToString("N")[..8]}";
            await PostgresFixture.InsertUserAsync(harness.AppDbConnectionString, id, $"{id}@example.com", tenantA);
        }

        string otherId = $"user_{Guid.NewGuid().ToString("N")[..8]}";
        await PostgresFixture.InsertUserAsync(harness.AppDbConnectionString, otherId, $"{otherId}@example.com", tenantB);

        PagedResult<AdminUserRecord> page = await harness.Users.SearchAsync(
            Slug, new UserSearchRequest { TenantId = tenantA, Page = 1, PageSize = 2 });

        page.TotalCount.Should().Be(3);
        page.Items.Should().HaveCount(2);
        page.Items.Should().OnlyContain(u => u.TenantId == tenantA);
    }

    [Test]
    public async Task SetActiveAsync_FlipsFlag_AndWritesAuditEntry()
    {
        ServiceHarness harness = await CreateHarnessAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string userId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(harness.AppDbConnectionString, tenantId, "Deactivate Tenant");
        await PostgresFixture.InsertUserAsync(harness.AppDbConnectionString, userId, $"{userId}@example.com", tenantId, isActive: true);

        bool changed = await harness.Users.SetActiveAsync(Slug, userId, isActive: false, reason: "fraud investigation");

        changed.Should().BeTrue();
        AdminUserRecord? user = await harness.Users.GetAsync(Slug, userId);
        user.Should().NotBeNull();
        user.IsActive.Should().BeFalse();

        PagedResult<AdminAuditEntryEntity> audit = await harness.AuditReader.SearchAsync(
            Slug, new AuditSearchRequest { ActionContains = "user.deactivate" });
        audit.Items.Should().Contain(e => e.TargetId == userId && e.Reason == "fraud investigation");
    }

    [Test]
    public async Task SetActiveAsync_UnknownUser_ReturnsFalse()
    {
        ServiceHarness harness = await CreateHarnessAsync();

        bool changed = await harness.Users.SetActiveAsync(Slug, "user_does_not_exist", isActive: false, reason: "cleanup");

        changed.Should().BeFalse();
    }

    [Test]
    public async Task SetActiveAsync_EmptyReason_Throws()
    {
        ServiceHarness harness = await CreateHarnessAsync();

        Func<Task> act = () => harness.Users.SetActiveAsync(Slug, "user_x", isActive: false, reason: " ");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task UserService_NonAdmin_Throws()
    {
        ServiceHarness harness = await CreateHarnessAsync(isAdmin: false);

        Func<Task> act = () => harness.Users.SearchAsync(Slug, new UserSearchRequest());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task TenantSearch_FiltersByName()
    {
        ServiceHarness harness = await CreateHarnessAsync();
        string marker = Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(
            harness.AppDbConnectionString, $"tenant_x_{marker}", $"Findable {marker} Org");
        await PostgresFixture.InsertTenantAsync(
            harness.AppDbConnectionString, $"tenant_y_{marker}", "Unrelated Org");

        PagedResult<AdminTenantRecord> result = await harness.Tenants.SearchAsync(
            Slug, nameContains: $"findable {marker}", page: 1, pageSize: 10);

        result.TotalCount.Should().Be(1);
        result.Items.Single().Name.Should().Be($"Findable {marker} Org");
    }

    [Test]
    public async Task Dashboard_CountsUsersTenantsAndRecentSignups()
    {
        ServiceHarness harness = await CreateHarnessAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(harness.AppDbConnectionString, tenantId, "Dashboard Tenant");

        string activeId = "user_" + Guid.NewGuid().ToString("N")[..8];
        string inactiveId = "user_" + Guid.NewGuid().ToString("N")[..8];
        string oldId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertUserAsync(harness.AppDbConnectionString, activeId, $"{activeId}@example.com", tenantId, isActive: true);
        await PostgresFixture.InsertUserAsync(harness.AppDbConnectionString, inactiveId, $"{inactiveId}@example.com", tenantId, isActive: false);
        await PostgresFixture.InsertUserAsync(
            harness.AppDbConnectionString, oldId, $"{oldId}@example.com", tenantId,
            createdAt: DateTime.UtcNow.AddDays(-90));

        DashboardStats stats = await harness.Dashboard.GetStatsAsync(Slug);

        stats.TotalUsers.Should().BeGreaterThanOrEqualTo(3);
        stats.ActiveUsers.Should().BeGreaterThanOrEqualTo(1);
        stats.TotalTenants.Should().BeGreaterThanOrEqualTo(1);
        stats.SignupsLast30Days.Should().BeGreaterThanOrEqualTo(2);
        stats.TotalUsers.Should().BeGreaterThan(stats.ActiveUsers);
    }
}
