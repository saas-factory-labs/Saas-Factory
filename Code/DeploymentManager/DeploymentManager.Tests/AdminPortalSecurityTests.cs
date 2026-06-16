using System.Reflection;
using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaaSFactory.Sample.Admin;

namespace DeploymentManager.Tests;

/// <summary>
/// Security regression tests (OWASP A01 - Broken Access Control) for the admin portal
/// shell: the kernel's generic pages and the sample plugin module must be fully
/// role-gated, route-confined and loadable through the real plugin pipeline.
/// External app repos (e.g. SaaSFactory.Dating.Admin) run the same inspector in their
/// own test suites - see the AdminPortalKernel README.
/// </summary>
internal sealed class AdminPortalSecurityTests
{
    private static readonly Assembly KernelAssembly = typeof(AdminPortalSecurityInspector).Assembly;
    private static readonly Assembly SampleModuleAssembly = typeof(SampleAdminModule).Assembly;

    [Test]
    public async Task KernelPages_MustRequireDeploymentManagerAdminRole()
    {
        string violations = string.Join(", ", AdminPortalSecurityInspector.FindUnprotectedRoutableComponents(KernelAssembly));
        await Assert.That(violations).IsEmpty();
    }

    [Test]
    public async Task KernelAssembly_MustNotContainAllowAnonymous()
    {
        string violations = string.Join(", ", AdminPortalSecurityInspector.FindAllowAnonymousUsages(KernelAssembly));
        await Assert.That(violations).IsEmpty();
    }

    [Test]
    public async Task SampleModule_MustPassFullSecurityInspection()
    {
        string violations = string.Join(", ", AdminPortalSecurityInspector.InspectModuleAssembly(SampleModuleAssembly, ["sample"]));
        await Assert.That(violations).IsEmpty();
    }

    [Test]
    public async Task SampleModule_LoadsThroughRealPluginPipeline()
    {
        // Flow 1 end-to-end: drop the dll in a folder, point the loader at it and the
        // module must come up registered, with its pages available to the router.
        string pluginsFolder = Path.Combine(Path.GetTempPath(), "dm-adminportal-e2e-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(pluginsFolder);
        try
        {
            string source = SampleModuleAssembly.Location;
            File.Copy(source, Path.Combine(pluginsFolder, Path.GetFileName(source)), overwrite: true);

            var services = new ServiceCollection();
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AdminPortal:Modules:sample:ConnectionString"] = "Host=localhost;Database=sample_app"
                })
                .Build();

            AdminPortalBuilder builder = services.AddAdminPortalKernel(configuration)
                .AddAdminPortalPlugins(pluginsFolder);

            await Assert.That(builder.Registry.TryGet("sample", out _)).IsTrue();
            await Assert.That(builder.Registry.RouterAssemblies.Contains(SampleModuleAssembly)).IsTrue();
            await Assert.That(builder.Registry.RouterAssemblies.Contains(KernelAssembly)).IsTrue();
        }
        finally
        {
            Directory.Delete(pluginsFolder, recursive: true);
        }
    }
}
