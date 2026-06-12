using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.SharedKernel.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.AdminPortalKernel.Tests.Integration;

internal sealed class AdminPortalAppDbContextTests
{
    private static AdminPortalAppDbContext CreateContext(string connectionString)
    {
        DbContextOptions<AdminPortalAppDbContext> options = new DbContextOptionsBuilder<AdminPortalAppDbContext>()
            .UseNpgsql(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .Options;
        return new AdminPortalAppDbContext(options);
    }

    [Test]
    public async Task Users_MappedColumns_RoundTrip()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string userId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(connectionString, tenantId, "Roundtrip Tenant");
        await PostgresFixture.InsertUserAsync(
            connectionString, userId, $"{userId}@example.com", tenantId,
            firstName: "Ada", lastName: "Lovelace", isActive: true);

        await using AdminPortalAppDbContext context = CreateContext(connectionString);
        AdminUserRecord? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);

        user.Should().NotBeNull();
        user.Email.Should().Be($"{userId}@example.com");
        user.FirstName.Should().Be("Ada");
        user.LastName.Should().Be("Lovelace");
        user.TenantId.Should().Be(tenantId);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeAfter(DateTime.MinValue);
    }

    [Test]
    public async Task Tenants_TenantType_MapsFromIntegerColumn()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(
            connectionString, tenantId, "Org Tenant",
            tenantType: (int)TenantType.Organization, email: "org@example.com", country: "DK", vatNumber: "DK12345678");

        await using AdminPortalAppDbContext context = CreateContext(connectionString);
        AdminTenantRecord? tenant = await context.Tenants.SingleOrDefaultAsync(t => t.Id == tenantId);

        tenant.Should().NotBeNull();
        tenant.TenantType.Should().Be(TenantType.Organization);
        tenant.Name.Should().Be("Org Tenant");
        tenant.Email.Should().Be("org@example.com");
        tenant.Country.Should().Be("DK");
        tenant.VatNumber.Should().Be("DK12345678");
    }

    [Test]
    public async Task SoftDeletedRows_AreExcludedByGlobalQueryFilter()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string deletedUserId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(connectionString, tenantId, "SoftDelete Tenant", isSoftDeleted: true);
        await PostgresFixture.InsertUserAsync(
            connectionString, deletedUserId, $"{deletedUserId}@example.com", tenantId, isSoftDeleted: true);

        await using AdminPortalAppDbContext context = CreateContext(connectionString);

        (await context.Users.AnyAsync(u => u.Id == deletedUserId)).Should().BeFalse();
        (await context.Tenants.AnyAsync(t => t.Id == tenantId)).Should().BeFalse();
    }

    [Test]
    public async Task ExecuteUpdate_FlipsIsActive_WithoutTracking()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string userId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(connectionString, tenantId, "Update Tenant");
        await PostgresFixture.InsertUserAsync(connectionString, userId, $"{userId}@example.com", tenantId, isActive: true);

        await using AdminPortalAppDbContext context = CreateContext(connectionString);
        int affected = await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.IsActive, false));

        affected.Should().Be(1);
        (await context.Users.SingleAsync(u => u.Id == userId)).IsActive.Should().BeFalse();
    }
}
