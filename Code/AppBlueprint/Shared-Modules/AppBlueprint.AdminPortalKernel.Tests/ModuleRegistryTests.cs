using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class ModuleRegistryTests
{
    [Test]
    public async Task Register_Then_TryGet_ReturnsModule()
    {
        var registry = new AdminPortalModuleRegistry();
        var module = new FixtureAdminModule();

        registry.Register(module);

        registry.TryGet("fixture-app", out IAdminPortalModule? found).Should().BeTrue();
        found.Should().BeSameAs(module);
        await Task.CompletedTask;
    }

    [Test]
    public async Task TryGet_UnknownSlug_ReturnsFalse()
    {
        var registry = new AdminPortalModuleRegistry();

        registry.TryGet("unknown", out IAdminPortalModule? found).Should().BeFalse();
        found.Should().BeNull();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Register_DuplicateSlug_Throws()
    {
        var registry = new AdminPortalModuleRegistry();
        registry.Register(new FixtureAdminModule());

        Action act = () => registry.Register(new DuplicateSlugAdminModule());

        act.Should().Throw<InvalidOperationException>().WithMessage("*fixture-app*");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Register_InvalidSlug_Throws()
    {
        var registry = new AdminPortalModuleRegistry();

        Action act = () => registry.Register(new BadSlugAdminModule());

        act.Should().Throw<ArgumentException>().WithMessage("*Bad Slug!*");
        await Task.CompletedTask;
    }

    [Test]
    public async Task Modules_ReturnsAllRegisteredModules()
    {
        var registry = new AdminPortalModuleRegistry();
        registry.Register(new FixtureAdminModule());
        registry.Register(new SecondFixtureAdminModule());

        registry.Modules.Should().HaveCount(2);
        registry.Modules.Select(m => m.Slug).Should().BeEquivalentTo("fixture-app", "second-app");
        await Task.CompletedTask;
    }

    [Test]
    public async Task RouterAssemblies_ContainsKernelAndModuleAssemblies_Distinct()
    {
        var registry = new AdminPortalModuleRegistry();
        // Both fixture modules live in the test assembly - RouterAssemblies must deduplicate.
        registry.Register(new FixtureAdminModule());
        registry.Register(new SecondFixtureAdminModule());

        registry.RouterAssemblies.Should().Contain(typeof(IAdminPortalModule).Assembly);
        registry.RouterAssemblies.Should().Contain(typeof(FixtureAdminModule).Assembly);
        registry.RouterAssemblies.Should().OnlyHaveUniqueItems();
        await Task.CompletedTask;
    }

    [Test]
    public async Task DefaultContractMembers_ProvideSensibleValues()
    {
        // Default interface members are only reachable through the interface type.
        IAdminPortalModule module = new FixtureAdminModule();

        module.RouterAssembly.Should().BeSameAs(typeof(FixtureAdminModule).Assembly);
        module.ExtraNavItems.Should().BeEmpty();
        module.Icon.Should().BeNull("modules without a custom icon use the shell's default");
        module.SiteUrl.Should().BeNull("modules without a public site hide the Visit site link");
        module.LocalSiteUrl.Should().BeNull();
        await Task.CompletedTask;
    }

    [Test]
    public async Task ResolveSiteUrl_InDevelopment_PrefersLocalSiteUrl()
    {
        IAdminPortalModule module = new SiteLinkedAdminModule();

        module.ResolveSiteUrl(isDevelopment: true).Should().Be("https://localhost:7247");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ResolveSiteUrl_InProduction_UsesPublicSiteUrl()
    {
        IAdminPortalModule module = new SiteLinkedAdminModule();

        module.ResolveSiteUrl(isDevelopment: false).Should().Be("https://site-app.example.com");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ResolveSiteUrl_InDevelopment_FallsBackToSiteUrl_WhenNoLocalUrl()
    {
        IAdminPortalModule module = new ProductionOnlyAdminModule();

        module.ResolveSiteUrl(isDevelopment: true).Should().Be("https://prod-app.example.com");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ResolveSiteUrl_ReturnsNull_WhenNoUrlsConfigured()
    {
        IAdminPortalModule module = new FixtureAdminModule();

        module.ResolveSiteUrl(isDevelopment: true).Should().BeNull();
        module.ResolveSiteUrl(isDevelopment: false).Should().BeNull();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Branding_DefaultsToNull()
    {
        IAdminPortalModule module = new FixtureAdminModule();

        module.Branding.Should().BeNull("unbranded modules use the shell's default violet");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ToInlineStyle_EmitsCssVariablesForSetTokens()
    {
        var branding = new AdminPortalBranding
        {
            PrimaryColor = "#016be9",
            PrimaryHoverColor = "#015bc6",
            OnPrimaryColor = "#ffffff",
            ButtonRadius = "0.5rem"
        };

        branding.ToInlineStyle().Should().Be(
            "--brand-primary:#016be9;--brand-primary-hover:#015bc6;--brand-on-primary:#ffffff;--brand-radius:0.5rem;");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ToInlineStyle_SkipsUnsetTokens()
    {
        var branding = new AdminPortalBranding { PrimaryColor = "#016be9" };

        branding.ToInlineStyle().Should().Be("--brand-primary:#016be9;");
        await Task.CompletedTask;
    }

    [Test]
    public async Task ToInlineStyle_DropsTokensThatCouldBreakOutOfTheStyleAttribute()
    {
        var branding = new AdminPortalBranding
        {
            PrimaryColor = "red; } body { display:none",
            ButtonRadius = "0.5rem"
        };

        // The injection attempt is dropped; the safe token still renders.
        branding.ToInlineStyle().Should().Be("--brand-radius:0.5rem;");
        await Task.CompletedTask;
    }
}
