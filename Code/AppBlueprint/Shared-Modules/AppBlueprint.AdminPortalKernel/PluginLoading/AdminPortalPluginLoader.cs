using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using AppBlueprint.AdminPortalKernel.Modules;

namespace AppBlueprint.AdminPortalKernel.PluginLoading;

/// <summary>
/// Loads admin portal plugin assemblies (e.g. SaaSFactory.Dating.Admin.dll) from a folder.
/// Assemblies are loaded into the default AssemblyLoadContext so plugin types share
/// identity with the host's kernel/MudBlazor types - a hard requirement for the module
/// contract and for Blazor rendering. Plugins must therefore be compiled against the
/// same kernel and MudBlazor versions as the host.
/// </summary>
public static class AdminPortalPluginLoader
{
    private static readonly ConcurrentDictionary<string, byte> HookedFolders = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Scans <paramref name="pluginsPath"/> for *.dll files, loads them and instantiates
    /// every concrete <see cref="IAdminPortalModule"/> implementation found.
    /// A missing folder yields an empty result; a module that cannot be instantiated throws.
    /// </summary>
    public static AdminPortalPluginLoadResult LoadFrom(string? pluginsPath)
    {
        if (string.IsNullOrWhiteSpace(pluginsPath))
        {
            return AdminPortalPluginLoadResult.Empty(folderFound: false);
        }

        string fullPath = Path.GetFullPath(pluginsPath);
        if (!Directory.Exists(fullPath))
        {
            return AdminPortalPluginLoadResult.Empty(folderFound: false);
        }

        EnsureDependencyResolutionHook(fullPath);

        List<Assembly> assemblies = new();
        List<IAdminPortalModule> modules = new();
        List<string> skippedFiles = new();

        foreach (string dllPath in Directory.EnumerateFiles(fullPath, "*.dll").OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            Assembly assembly;
            try
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dllPath);
            }
            catch (BadImageFormatException ex)
            {
                skippedFiles.Add($"{dllPath}: {ex.Message}");
                continue;
            }
            catch (FileLoadException ex)
            {
                skippedFiles.Add($"{dllPath}: {ex.Message}");
                continue;
            }

            assemblies.Add(assembly);
            modules.AddRange(CreateModules(assembly));
        }

        return new AdminPortalPluginLoadResult(folderFound: true, assemblies, modules, skippedFiles);
    }

    private static List<IAdminPortalModule> CreateModules(Assembly assembly)
    {
        List<IAdminPortalModule> modules = new();

        foreach (Type moduleType in GetLoadableTypes(assembly)
                     .Where(type => typeof(IAdminPortalModule).IsAssignableFrom(type)
                                    && type is { IsClass: true, IsAbstract: false }))
        {
            object? instance = Activator.CreateInstance(moduleType);
            if (instance is not IAdminPortalModule module)
            {
                throw new InvalidOperationException(
                    $"Admin portal module type '{moduleType.FullName}' could not be instantiated.");
            }

            modules.Add(module);
        }

        return modules;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            // Partially loadable assemblies (e.g. optional dependencies missing) still
            // expose the types that did load; module discovery works on those.
            return ex.Types.Where(type => type is not null)!;
        }
    }

    /// <summary>
    /// Plugin-private dependencies sit next to the plugin dll. The default load context
    /// only probes the host's output folder, so fall back to the plugins folder for
    /// anything the host does not provide.
    /// </summary>
    private static void EnsureDependencyResolutionHook(string pluginsFolder)
    {
        if (!HookedFolders.TryAdd(pluginsFolder, 0))
        {
            return;
        }

        AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
        {
            string candidate = Path.Combine(pluginsFolder, assemblyName.Name + ".dll");
            return File.Exists(candidate) ? context.LoadFromAssemblyPath(candidate) : null;
        };
    }
}
