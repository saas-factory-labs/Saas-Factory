using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class AdminPortalOptionsValidationTests
{
    private static AdminPortalModuleRegistry CreateRegistryWithFixtureModule()
    {
        var registry = new AdminPortalModuleRegistry();
        registry.Register(new FixtureAdminModule());
        return registry;
    }

    [Test]
    public async Task Validate_RegisteredModuleWithConnectionString_Succeeds()
    {
        var validator = new AdminPortalOptionsValidator(CreateRegistryWithFixtureModule());
        var options = new AdminPortalOptions();
        options.Modules["fixture-app"] = new AdminPortalModuleOptions
        {
            ConnectionString = "Host=localhost;Database=fixture;Username=admin;Password=secret"
        };

        ValidateOptionsResult result = validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Validate_RegisteredModuleWithoutConnectionString_Fails()
    {
        var validator = new AdminPortalOptionsValidator(CreateRegistryWithFixtureModule());
        var options = new AdminPortalOptions();

        ValidateOptionsResult result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("fixture-app");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Validate_RegisteredModuleWithEmptyConnectionString_Fails()
    {
        var validator = new AdminPortalOptionsValidator(CreateRegistryWithFixtureModule());
        var options = new AdminPortalOptions();
        options.Modules["fixture-app"] = new AdminPortalModuleOptions { ConnectionString = "  " };

        ValidateOptionsResult result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("fixture-app");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Validate_ExtraConfiguredModulesWithoutRegistration_AreIgnored()
    {
        var validator = new AdminPortalOptionsValidator(CreateRegistryWithFixtureModule());
        var options = new AdminPortalOptions();
        options.Modules["fixture-app"] = new AdminPortalModuleOptions { ConnectionString = "Host=x;Database=y" };
        options.Modules["not-installed-yet"] = new AdminPortalModuleOptions { ConnectionString = "Host=a;Database=b" };

        ValidateOptionsResult result = validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
        await Task.CompletedTask;
    }
}
