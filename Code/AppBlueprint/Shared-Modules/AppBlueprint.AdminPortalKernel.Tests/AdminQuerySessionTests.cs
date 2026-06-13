using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Domain;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Services;
using AppBlueprint.AdminPortalKernel.Tests.Integration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class AdminQuerySessionTests
{
    private static IAdminPortalUserContext CreateAdminUserContext(bool isAdmin = true)
    {
        IAdminPortalUserContext userContext = Substitute.For<IAdminPortalUserContext>();
        userContext.IsDeploymentManagerAdminAsync().Returns(isAdmin);
        userContext.GetUserIdAsync().Returns("admin_user_1");
        userContext.GetEmailAsync().Returns("owner@example.com");
        return userContext;
    }

    private static AdminPortalDbContextFactory CreateFactory(string connectionString)
    {
        var options = new AdminPortalOptions();
        options.Modules["fixture-app"] = new AdminPortalModuleOptions { ConnectionString = connectionString };
        return new AdminPortalDbContextFactory(Options.Create(options));
    }

    [Test]
    public async Task ExecuteReadAsync_NonAdmin_ThrowsUnauthorized()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory("Host=unused;Database=unused");
        var session = new AdminQuerySession(factory, CreateAdminUserContext(isAdmin: false), NullLogger<AdminQuerySession>.Instance);

        Func<Task> act = () => session.ExecuteReadAsync(
            "fixture-app", "list users", context => context.Users.CountAsync());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ExecuteReadAsync_EmptyReason_Throws()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory("Host=unused;Database=unused");
        var session = new AdminQuerySession(factory, CreateAdminUserContext(), NullLogger<AdminQuerySession>.Instance);

        Func<Task> act = () => session.ExecuteReadAsync(
            "fixture-app", "   ", context => context.Users.CountAsync());

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task ExecuteWriteAsync_NonAdmin_ThrowsUnauthorized()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory("Host=unused;Database=unused");
        var session = new AdminQuerySession(factory, CreateAdminUserContext(isAdmin: false), NullLogger<AdminQuerySession>.Instance);

        Func<Task> act = () => session.ExecuteWriteAsync(
            "fixture-app", "deactivate user", "tenant_1", context => Task.FromResult(0));

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task ExecuteReadAsync_AsAdmin_RunsQueryAgainstModuleDatabase()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string userId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(connectionString, tenantId, "Session Tenant");
        await PostgresFixture.InsertUserAsync(connectionString, userId, $"{userId}@example.com", tenantId);

        await using AdminPortalDbContextFactory factory = CreateFactory(connectionString);
        var session = new AdminQuerySession(factory, CreateAdminUserContext(), NullLogger<AdminQuerySession>.Instance);

        AdminUserRecord? user = await session.ExecuteReadAsync(
            "fixture-app", "load user for test",
            context => context.Users.SingleOrDefaultAsync(u => u.Id == userId));

        user.Should().NotBeNull();
        user.Email.Should().Be($"{userId}@example.com");
    }

    [Test]
    public async Task ExecuteWriteAsync_AsAdmin_AppliesUpdate()
    {
        string connectionString = await PostgresFixture.GetConnectionStringAsync();
        string tenantId = "tenant_" + Guid.NewGuid().ToString("N")[..8];
        string userId = "user_" + Guid.NewGuid().ToString("N")[..8];
        await PostgresFixture.InsertTenantAsync(connectionString, tenantId, "Write Tenant");
        await PostgresFixture.InsertUserAsync(connectionString, userId, $"{userId}@example.com", tenantId, isActive: true);

        await using AdminPortalDbContextFactory factory = CreateFactory(connectionString);
        var session = new AdminQuerySession(factory, CreateAdminUserContext(), NullLogger<AdminQuerySession>.Instance);

        int affected = await session.ExecuteWriteAsync(
            "fixture-app", "deactivate user for test", tenantId,
            context => context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.IsActive, false)));

        affected.Should().Be(1);
    }
}
