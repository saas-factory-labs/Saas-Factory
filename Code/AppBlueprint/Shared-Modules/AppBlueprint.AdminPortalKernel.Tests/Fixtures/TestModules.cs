using AppBlueprint.AdminPortalKernel.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.AdminPortalKernel.Tests.Fixtures;

/// <summary>
/// Minimal valid module - relies on the contract's default members
/// (RouterAssembly, Icon, ExtraNavItems, ConfigureServices).
/// </summary>
public sealed class FixtureAdminModule : IAdminPortalModule
{
    public string Slug => "fixture-app";
    public string DisplayName => "Fixture App";
}

/// <summary>Second valid module used for multi-module registry tests.</summary>
public sealed class SecondFixtureAdminModule : IAdminPortalModule
{
    public string Slug => "second-app";
    public string DisplayName => "Second App";

    public IReadOnlyList<AdminPortalNavItem> ExtraNavItems { get; } =
    [
        new AdminPortalNavItem("Custom Page", "/apps/second-app/admin/custom", "extension")
    ];

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<SecondModuleMarkerService>();
    }
}

/// <summary>Marker service used to verify that ConfigureServices was invoked.</summary>
public sealed class SecondModuleMarkerService;

/// <summary>Module with an invalid slug (uppercase + space) - must be rejected at registration.</summary>
public sealed class BadSlugAdminModule : IAdminPortalModule
{
    public string Slug => "Bad Slug!";
    public string DisplayName => "Bad Slug App";
}

/// <summary>Module that duplicates the slug of <see cref="FixtureAdminModule"/>.</summary>
public sealed class DuplicateSlugAdminModule : IAdminPortalModule
{
    public string Slug => "fixture-app";
    public string DisplayName => "Duplicate Fixture App";
}
