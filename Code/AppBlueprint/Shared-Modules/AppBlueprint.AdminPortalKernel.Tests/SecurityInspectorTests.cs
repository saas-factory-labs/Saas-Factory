using System.Reflection;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Testing;
using AppBlueprint.AdminPortalKernel.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.AdminPortalKernel.Tests;

internal sealed class SecurityInspectorTests
{
    private static readonly Assembly FixtureAssembly = typeof(UnprotectedFixtureController).Assembly;

    [Test]
    public async Task FindUnprotectedControllers_FlagsMissingAndRolelessAuthorize()
    {
        IReadOnlyList<string> violations = AdminPortalSecurityInspector.FindUnprotectedControllers(FixtureAssembly);

        violations.Should().Contain(v => v.Contains(nameof(UnprotectedFixtureController), StringComparison.Ordinal));
        violations.Should().Contain(v => v.Contains(nameof(RolelessFixtureController), StringComparison.Ordinal));
        violations.Should().NotContain(v => v.Contains(nameof(AnonymousLeakFixtureController), StringComparison.Ordinal));
    }

    [Test]
    public async Task FindAllowAnonymousUsages_FlagsLeakedAction()
    {
        IReadOnlyList<string> violations = AdminPortalSecurityInspector.FindAllowAnonymousUsages(FixtureAssembly);

        violations.Should().Contain(v =>
            v.Contains(nameof(AnonymousLeakFixtureController), StringComparison.Ordinal)
            && v.Contains("Leak", StringComparison.Ordinal));
        await Task.CompletedTask;
    }

    [Test]
    public async Task FindUnprotectedRoutableComponents_FlagsComponentWithoutRole()
    {
        IReadOnlyList<string> violations = AdminPortalSecurityInspector.FindUnprotectedRoutableComponents(FixtureAssembly);

        violations.Should().Contain(v => v.Contains(nameof(UnprotectedFixtureComponent), StringComparison.Ordinal));
        violations.Should().NotContain(v => v.Contains(nameof(GoodFixtureComponent), StringComparison.Ordinal));
        await Task.CompletedTask;
    }

    [Test]
    public async Task FindRoutesOutsideSlugPrefixes_FlagsEscapedRoute()
    {
        IReadOnlyList<string> violations = AdminPortalSecurityInspector.FindRoutesOutsideSlugPrefixes(
            FixtureAssembly, ["fixture-app"]);

        violations.Should().Contain(v => v.Contains("/totally/elsewhere", StringComparison.Ordinal));
        violations.Should().NotContain(v => v.Contains("/apps/fixture-app/admin/good", StringComparison.Ordinal));
        await Task.CompletedTask;
    }

    [Test]
    public async Task KernelAssembly_HasNoControllerOrAnonymousViolations()
    {
        Assembly kernelAssembly = typeof(AdminPortalSecurityInspector).Assembly;

        AdminPortalSecurityInspector.FindUnprotectedControllers(kernelAssembly).Should().BeEmpty();
        AdminPortalSecurityInspector.FindAllowAnonymousUsages(kernelAssembly).Should().BeEmpty();
        AdminPortalSecurityInspector.FindUnprotectedRoutableComponents(kernelAssembly).Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Test]
    public async Task AddAdminPortalPlugins_WithViolatingPluginAssembly_FailsStartup()
    {
        // The test assembly doubles as a hostile plugin: it contains the deliberately
        // insecure fixture types above, so loading it through the plugin pipeline
        // must refuse to start and must not register any module.
        string pluginsFolder = Path.Combine(Path.GetTempPath(), "adminportal-hostile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pluginsFolder);
        try
        {
            string source = FixtureAssembly.Location;
            File.Copy(source, Path.Combine(pluginsFolder, Path.GetFileName(source)), overwrite: true);

            var services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder().Build();
            AdminPortalBuilder builder = services.AddAdminPortalKernel(configuration);

            Action act = () => builder.AddAdminPortalPlugins(pluginsFolder);

            act.Should().Throw<InvalidOperationException>().WithMessage("*security violations*");
            builder.Registry.Modules.Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(pluginsFolder, recursive: true);
        }

        await Task.CompletedTask;
    }
}
