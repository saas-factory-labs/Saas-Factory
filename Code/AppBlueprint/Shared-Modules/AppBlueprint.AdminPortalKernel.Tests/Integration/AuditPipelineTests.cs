using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Domain.Dtos;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests.Integration;

internal sealed class AuditPipelineTests
{
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

    private static readonly SemaphoreSlim SchemaLock = new(1, 1);
    private static bool _schemaCreated;

    private static async Task<TestAuditContextFactory> CreateFactoryAsync()
    {
        string connectionString = await PostgresFixture.GetAuditConnectionStringAsync();
        var factory = new TestAuditContextFactory(connectionString);

        await SchemaLock.WaitAsync();
        try
        {
            if (!_schemaCreated)
            {
                await using AdminPortalAuditDbContext context = factory.CreateDbContext();
                await context.Database.EnsureCreatedAsync();
                _schemaCreated = true;
            }
        }
        finally
        {
            SchemaLock.Release();
        }

        return factory;
    }

    private static IAdminPortalUserContext CreateAdminUserContext(bool isAdmin = true)
    {
        IAdminPortalUserContext userContext = Substitute.For<IAdminPortalUserContext>();
        userContext.IsDeploymentManagerAdminAsync().Returns(isAdmin);
        userContext.GetUserIdAsync().Returns("admin_user_1");
        userContext.GetEmailAsync().Returns("owner@example.com");
        return userContext;
    }

    [Test]
    public async Task WriteAsync_PersistsEntry_ReadableViaReader()
    {
        TestAuditContextFactory factory = await CreateFactoryAsync();
        string slug = "app-" + Guid.NewGuid().ToString("N")[..8];
        var writer = new AdminAuditWriter(factory, CreateAdminUserContext());
        var reader = new AdminAuditReader(factory, CreateAdminUserContext());

        AdminAuditEntryEntity written = await writer.WriteAsync(
            slug, action: "user.deactivate", reason: "fraud suspicion",
            targetType: "User", targetId: "user_123", tenantId: "tenant_456");

        written.Id.Should().StartWith("audit_");
        written.AdminEmail.Should().Be("owner@example.com");

        PagedResult<AdminAuditEntryEntity> page = await reader.SearchAsync(slug, new AuditSearchRequest());
        page.TotalCount.Should().Be(1);
        page.Items.Single().Action.Should().Be("user.deactivate");
        page.Items.Single().Reason.Should().Be("fraud suspicion");
        page.Items.Single().TargetId.Should().Be("user_123");
    }

    [Test]
    public async Task WriteAsync_MissingReason_Throws()
    {
        TestAuditContextFactory factory = await CreateFactoryAsync();
        var writer = new AdminAuditWriter(factory, CreateAdminUserContext());

        Func<Task> act = () => writer.WriteAsync("some-app", "user.view", reason: "  ");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task WriteAsync_NonAdmin_Throws()
    {
        TestAuditContextFactory factory = await CreateFactoryAsync();
        var writer = new AdminAuditWriter(factory, CreateAdminUserContext(isAdmin: false));

        Func<Task> act = () => writer.WriteAsync("some-app", "user.view", reason: "support case");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task SearchAsync_FiltersBySlugActionAndDateRange_WithPaging()
    {
        TestAuditContextFactory factory = await CreateFactoryAsync();
        string slug = "app-" + Guid.NewGuid().ToString("N")[..8];
        string otherSlug = "app-" + Guid.NewGuid().ToString("N")[..8];
        var writer = new AdminAuditWriter(factory, CreateAdminUserContext());
        var reader = new AdminAuditReader(factory, CreateAdminUserContext());

        for (int i = 0; i < 3; i++)
        {
            await writer.WriteAsync(slug, "user.deactivate", $"bulk cleanup {i}");
        }

        await writer.WriteAsync(slug, "tenant.view", "inspection");
        await writer.WriteAsync(otherSlug, "user.deactivate", "other app entry");

        PagedResult<AdminAuditEntryEntity> filtered = await reader.SearchAsync(
            slug, new AuditSearchRequest { ActionContains = "deactivate", Page = 1, PageSize = 2 });

        filtered.TotalCount.Should().Be(3);
        filtered.Items.Should().HaveCount(2);
        filtered.Items.Should().OnlyContain(e => e.AppSlug == slug && e.Action == "user.deactivate");

        PagedResult<AdminAuditEntryEntity> outsideDateRange = await reader.SearchAsync(
            slug, new AuditSearchRequest { ToUtc = DateTime.UtcNow.AddDays(-1) });
        outsideDateRange.TotalCount.Should().Be(0);
    }

    [Test]
    public async Task GetRecentAsync_ReturnsNewestFirst()
    {
        TestAuditContextFactory factory = await CreateFactoryAsync();
        string slug = "app-" + Guid.NewGuid().ToString("N")[..8];
        var writer = new AdminAuditWriter(factory, CreateAdminUserContext());
        var reader = new AdminAuditReader(factory, CreateAdminUserContext());

        await writer.WriteAsync(slug, "first.action", "first");
        await writer.WriteAsync(slug, "second.action", "second");

        IReadOnlyList<AdminAuditEntryEntity> recent = await reader.GetRecentAsync(slug, count: 1);

        recent.Should().HaveCount(1);
        recent[0].Action.Should().Be("second.action");
    }
}
