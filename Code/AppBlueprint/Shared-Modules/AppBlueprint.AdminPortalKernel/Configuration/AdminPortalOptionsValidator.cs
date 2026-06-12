using AppBlueprint.AdminPortalKernel.Modules;
using Microsoft.Extensions.Options;

namespace AppBlueprint.AdminPortalKernel.Configuration;

/// <summary>
/// Fail-fast startup validation: every registered module must have a connection string
/// configured. An internal ops tool should refuse to boot misconfigured rather than
/// render broken admin portals.
/// </summary>
public sealed class AdminPortalOptionsValidator : IValidateOptions<AdminPortalOptions>
{
    private readonly AdminPortalModuleRegistry _registry;

    public AdminPortalOptionsValidator(AdminPortalModuleRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        _registry = registry;
    }

    public ValidateOptionsResult Validate(string? name, AdminPortalOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        List<string> failures = new();

        foreach (IAdminPortalModule module in _registry.Modules)
        {
            if (!options.Modules.TryGetValue(module.Slug, out AdminPortalModuleOptions? moduleOptions)
                || string.IsNullOrWhiteSpace(moduleOptions.ConnectionString))
            {
                failures.Add(
                    $"Admin portal module '{module.Slug}' has no connection string. " +
                    $"Configure AdminPortal:Modules:{module.Slug}:ConnectionString " +
                    $"(environment variable AdminPortal__Modules__{module.Slug}__ConnectionString).");
            }
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
