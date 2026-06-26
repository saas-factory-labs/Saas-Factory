using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using Pulumi;

namespace AppBlueprint.Infrastructure.Core.Providers;

/// <summary>
/// Shared base for <see cref="IInfrastructureProvider"/> implementations. Centralizes resource
/// naming, standard tagging, and — crucially — the policy for ignoring portal-managed secrets, so
/// secrets configured by hand in a cloud provider's dashboard are never overwritten or deleted on
/// the next <c>pulumi up</c>.
/// </summary>
public abstract class ProviderBase : IInfrastructureProvider
{
    /// <summary>
    /// Property names commonly used for portal-managed secrets on container/worker resources.
    /// Applied via <see cref="IgnorePortalManagedSecrets"/> so these are never reverted by Pulumi.
    /// </summary>
    public static readonly IReadOnlyList<string> DefaultPortalManagedSecretProperties =
        ["secretEnvironmentVariables", "vars"];

    /// <inheritdoc />
    public abstract CloudProvider Provider { get; }

    /// <inheritdoc />
    public abstract AppDeploymentOutputs Deploy(AppInfrastructureConfig config);

    /// <summary>Builds a stable, hyphenated resource name of the form <c>{appId}-{component}</c>.</summary>
    protected static string ResourceName(string appId, string component)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appId);
        ArgumentException.ThrowIfNullOrWhiteSpace(component);
        return $"{appId}-{component}";
    }

    /// <summary>Standard tags applied to every resource for traceability and cost attribution.</summary>
    protected static IReadOnlyDictionary<string, string> StandardTags(AppInfrastructureConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["app"] = config.AppId,
            ["provider"] = config.Provider.ToString(),
            ["provisionedBy"] = "AppBlueprint.Infrastructure.Core"
        };
    }

    /// <summary>
    /// Returns <see cref="CustomResourceOptions"/> instructing Pulumi to ignore changes to the given
    /// property names, so portal-managed secrets are never reverted on the next deployment. Falls
    /// back to <see cref="DefaultPortalManagedSecretProperties"/> when no names are supplied.
    /// </summary>
    protected static CustomResourceOptions IgnorePortalManagedSecrets(params string[] propertyNames)
    {
        IReadOnlyList<string> names = propertyNames is { Length: > 0 }
            ? propertyNames
            : DefaultPortalManagedSecretProperties;

        return new CustomResourceOptions { IgnoreChanges = names.ToList() };
    }
}
