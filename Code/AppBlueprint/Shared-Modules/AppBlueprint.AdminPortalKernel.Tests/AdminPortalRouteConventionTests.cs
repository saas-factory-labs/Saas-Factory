using System.Reflection;
using AppBlueprint.AdminPortalKernel.Testing;
using FluentAssertions;
using ComponentRouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace AppBlueprint.AdminPortalKernel.Tests;

/// <summary>
/// Security regression tests for the kernel's own UI pages (OWASP A01): every routable
/// page must carry the DeploymentManagerAdmin role gate and live under /apps/.
/// </summary>
internal sealed class AdminPortalRouteConventionTests
{
    private static readonly Assembly KernelAssembly = typeof(AdminPortalSecurityInspector).Assembly;

    [Test]
    public async Task KernelPages_AreRoleGated_AndFreeOfAllowAnonymous()
    {
        AdminPortalSecurityInspector.FindUnprotectedRoutableComponents(KernelAssembly).Should().BeEmpty();
        AdminPortalSecurityInspector.FindAllowAnonymousUsages(KernelAssembly).Should().BeEmpty();
        await Task.CompletedTask;
    }

    [Test]
    public async Task KernelPages_Exist_AndAllRoutesLiveUnderApps()
    {
        List<string> routeTemplates = KernelAssembly.GetTypes()
            .SelectMany(type => type.GetCustomAttributes<ComponentRouteAttribute>(inherit: false))
            .Select(route => route.Template)
            .ToList();

        routeTemplates.Should().Contain("/apps/{AppSlug}/admin");
        routeTemplates.Should().Contain("/apps/{AppSlug}/admin/users");
        routeTemplates.Should().Contain("/apps/{AppSlug}/admin/tenants");
        routeTemplates.Should().Contain("/apps/{AppSlug}/admin/audit");
        routeTemplates.Should().OnlyContain(template => template.StartsWith("/apps/", StringComparison.Ordinal));
        await Task.CompletedTask;
    }
}
