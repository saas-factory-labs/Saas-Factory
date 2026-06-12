using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class AdminPortalDbContextFactoryTests
{
    private static AdminPortalDbContextFactory CreateFactory()
    {
        var options = new AdminPortalOptions();
        options.Modules["fixture-app"] = new AdminPortalModuleOptions
        {
            ConnectionString = "Host=localhost;Port=5432;Database=fixture_db;Username=admin;Password=secret"
        };
        options.Modules["second-app"] = new AdminPortalModuleOptions
        {
            ConnectionString = "Host=localhost;Port=5432;Database=second_db;Username=admin;Password=secret"
        };
        return new AdminPortalDbContextFactory(Options.Create(options));
    }

    [Test]
    public async Task CreateForModule_UnknownSlug_Throws()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory();

        Action act = () => factory.CreateForModule("unknown-app");

        act.Should().Throw<InvalidOperationException>().WithMessage("*unknown-app*");
        await Task.CompletedTask;
    }

    [Test]
    public async Task CreateForModule_UsesModuleSpecificConnectionString()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory();

        await using AdminPortalAppDbContext first = factory.CreateForModule("fixture-app");
        await using AdminPortalAppDbContext second = factory.CreateForModule("second-app");

        first.Database.GetConnectionString().Should().Contain("fixture_db");
        second.Database.GetConnectionString().Should().Contain("second_db");
    }

    [Test]
    public async Task CreateForModule_RepeatedCalls_ReuseCachedConfiguration()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory();

        await using AdminPortalAppDbContext first = factory.CreateForModule("fixture-app");
        await using AdminPortalAppDbContext second = factory.CreateForModule("fixture-app");

        second.Should().NotBeSameAs(first);
        second.Database.GetConnectionString().Should().Be(first.Database.GetConnectionString());
    }

    [Test]
    public async Task CreateForModule_ContextsDefaultToNoTracking()
    {
        await using AdminPortalDbContextFactory factory = CreateFactory();

        await using AdminPortalAppDbContext context = factory.CreateForModule("fixture-app");

        context.ChangeTracker.QueryTrackingBehavior.Should().Be(QueryTrackingBehavior.NoTracking);
    }
}
