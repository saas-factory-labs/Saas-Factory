using AppBlueprint.AdminPortalKernel.Modules;
using AppBlueprint.AdminPortalKernel.PluginLoading;
using AppBlueprint.AdminPortalKernel.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>
/// Fluent builder returned by AddAdminPortalKernel, used to register admin portal modules
/// either at compile time or from a runtime plugins folder.
/// </summary>
public sealed class AdminPortalBuilder
{
    internal AdminPortalBuilder(IServiceCollection services, AdminPortalModuleRegistry registry)
    {
        Services = services;
        Registry = registry;
    }

    public IServiceCollection Services { get; }

    public AdminPortalModuleRegistry Registry { get; }

    /// <summary>Registers a module known at compile time (used by tests and first-party hosts).</summary>
    public AdminPortalBuilder AddAdminPortalModule<TModule>()
        where TModule : class, IAdminPortalModule, new()
    {
        var module = new TModule();
        Registry.Register(module);
        module.ConfigureServices(Services);
        return this;
    }

    /// <summary>
    /// Loads plugin dlls from <paramref name="pluginsPath"/>, runs the security inspector
    /// over every loaded assembly and registers every module found. A missing folder is
    /// allowed (zero plugins); a security violation or invalid module FAILS STARTUP -
    /// a misconfigured internal admin tool must refuse to boot rather than expose data.
    /// </summary>
    public AdminPortalBuilder AddAdminPortalPlugins(string? pluginsPath)
    {
        AdminPortalPluginLoadResult result = AdminPortalPluginLoader.LoadFrom(pluginsPath);

        List<string> violations = new();

        foreach (System.Reflection.Assembly assembly in result.Assemblies)
        {
            violations.AddRange(AdminPortalSecurityInspector.FindUnprotectedControllers(assembly));
            violations.AddRange(AdminPortalSecurityInspector.FindAllowAnonymousUsages(assembly));
            violations.AddRange(AdminPortalSecurityInspector.FindUnprotectedRoutableComponents(assembly));
        }

        foreach (var assemblyModules in result.Modules.GroupBy(module => module.RouterAssembly))
        {
            List<string> slugs = assemblyModules.Select(module => module.Slug).ToList();
            violations.AddRange(AdminPortalSecurityInspector.FindRoutesOutsideSlugPrefixes(assemblyModules.Key, slugs));
        }

        if (violations.Count > 0)
        {
            throw new InvalidOperationException(
                "Admin portal plugin security violations detected - refusing to start:" +
                Environment.NewLine + string.Join(Environment.NewLine, violations));
        }

        foreach (IAdminPortalModule module in result.Modules)
        {
            Registry.Register(module);
            module.ConfigureServices(Services);
        }

        LastPluginLoadResult = result;
        return this;
    }

    /// <summary>Result of the most recent plugin scan, exposed so the host can log it.</summary>
    public AdminPortalPluginLoadResult? LastPluginLoadResult { get; private set; }
}
