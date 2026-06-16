using AppBlueprint.AdminPortalKernel.Configuration;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildConfiguration(params KeyValuePair<string, string?>[] values)
    {
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Test]
    public async Task AddAdminPortalKernel_RegistersRegistryAsSingleton()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = BuildConfiguration();

        services.AddAdminPortalKernel(configuration);
        using ServiceProvider provider = services.BuildServiceProvider();

        provider.GetRequiredService<AdminPortalModuleRegistry>()
            .Should().BeSameAs(provider.GetRequiredService<AdminPortalModuleRegistry>());
        await Task.CompletedTask;
    }

    [Test]
    public async Task AddAdminPortalModule_RegistersModuleAndInvokesConfigureServices()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>("AdminPortal:Modules:second-app:ConnectionString", "Host=x;Database=y"));

        services.AddAdminPortalKernel(configuration)
            .AddAdminPortalModule<SecondFixtureAdminModule>();

        using ServiceProvider provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<AdminPortalModuleRegistry>();

        registry.TryGet("second-app", out _).Should().BeTrue();
        provider.GetService<SecondModuleMarkerService>().Should().NotBeNull();
        await Task.CompletedTask;
    }

    [Test]
    public async Task AddAdminPortalKernel_BindsOptionsFromConfiguration()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = BuildConfiguration(
            new KeyValuePair<string, string?>("AdminPortal:PluginsPath", "some/plugins"),
            new KeyValuePair<string, string?>("AdminPortal:Modules:fixture-app:ConnectionString", "Host=x;Database=y"));

        services.AddAdminPortalKernel(configuration).AddAdminPortalModule<FixtureAdminModule>();
        using ServiceProvider provider = services.BuildServiceProvider();

        AdminPortalOptions options = provider.GetRequiredService<IOptions<AdminPortalOptions>>().Value;
        options.PluginsPath.Should().Be("some/plugins");
        options.Modules.Should().ContainKey("fixture-app");
        options.Modules["fixture-app"].ConnectionString.Should().Be("Host=x;Database=y");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ResolvingValidatedOptions_WithMissingConnectionString_Throws()
    {
        var services = new ServiceCollection();
        IConfiguration configuration = BuildConfiguration();

        services.AddAdminPortalKernel(configuration).AddAdminPortalModule<FixtureAdminModule>();
        using ServiceProvider provider = services.BuildServiceProvider();

        Action act = () => _ = provider.GetRequiredService<IOptions<AdminPortalOptions>>().Value;

        act.Should().Throw<OptionsValidationException>().WithMessage("*fixture-app*");
        await Task.CompletedTask;
    }
}
