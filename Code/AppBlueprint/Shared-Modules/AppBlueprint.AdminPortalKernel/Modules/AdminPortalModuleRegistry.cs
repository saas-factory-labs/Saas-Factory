using System.Reflection;
using System.Text.RegularExpressions;

namespace AppBlueprint.AdminPortalKernel.Modules;

/// <summary>
/// Holds every admin portal module known to the shell. Populated once during startup
/// (compile-time registrations and/or runtime-loaded plugins) and read-only afterwards.
/// </summary>
public sealed partial class AdminPortalModuleRegistry
{
    private readonly Lock _lock = new();
    private readonly Dictionary<string, IAdminPortalModule> _modulesBySlug = new(StringComparer.Ordinal);

    [GeneratedRegex("^[a-z0-9-]+$")]
    private static partial Regex SlugPattern();

    /// <summary>Registers a module. Throws when the slug is invalid or already taken.</summary>
    public void Register(IAdminPortalModule module)
    {
        ArgumentNullException.ThrowIfNull(module);
        ArgumentException.ThrowIfNullOrWhiteSpace(module.Slug);

        if (!SlugPattern().IsMatch(module.Slug))
        {
            throw new ArgumentException(
                $"Admin portal module slug '{module.Slug}' ({module.GetType().FullName}) is invalid. " +
                "Slugs must contain only lowercase letters, digits and hyphens.",
                nameof(module));
        }

        lock (_lock)
        {
            if (_modulesBySlug.ContainsKey(module.Slug))
            {
                throw new InvalidOperationException(
                    $"An admin portal module with slug '{module.Slug}' is already registered. " +
                    $"Conflicting module: {module.GetType().FullName}.");
            }

            _modulesBySlug.Add(module.Slug, module);
        }
    }

    /// <summary>Looks up a module by its slug.</summary>
    public bool TryGet(string slug, out IAdminPortalModule? module)
    {
        ArgumentNullException.ThrowIfNull(slug);

        lock (_lock)
        {
            return _modulesBySlug.TryGetValue(slug, out module);
        }
    }

    /// <summary>All registered modules, ordered by display name for stable navigation rendering.</summary>
    public IReadOnlyList<IAdminPortalModule> GetModules()
    {
        lock (_lock)
        {
            return _modulesBySlug.Values.OrderBy(m => m.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }

    /// <summary>
    /// Assemblies the shell Router needs for route discovery:
    /// the kernel assembly (generic admin pages) plus every module's router assembly.
    /// </summary>
    public IReadOnlyList<Assembly> GetRouterAssemblies()
    {
        lock (_lock)
        {
            return new[] { typeof(IAdminPortalModule).Assembly }
                .Concat(_modulesBySlug.Values.Select(m => m.RouterAssembly))
                .Distinct()
                .ToList();
        }
    }
}
