using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;

namespace AppBlueprint.Infrastructure.Core;

/// <summary>
/// Selects the registered <see cref="IInfrastructureProvider"/> matching an application's configured
/// <see cref="CloudProvider"/> and delegates provisioning to it. Adding support for a new platform is
/// done purely by registering another <see cref="IInfrastructureProvider"/> — this factory itself
/// never needs to change.
/// </summary>
public sealed class AppStackFactory
{
    private readonly IReadOnlyDictionary<CloudProvider, IInfrastructureProvider> _providers;

    /// <summary>Creates a factory over the available providers, keyed by the platform each targets.</summary>
    /// <param name="providers">All registered providers; each must target a distinct platform.</param>
    public AppStackFactory(IEnumerable<IInfrastructureProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);
        _providers = providers.ToDictionary(provider => provider.Provider);
    }

    /// <summary>
    /// Provisions the application described by <paramref name="config"/> using the provider that
    /// targets its configured platform. Invoked from inside a Pulumi program.
    /// </summary>
    /// <exception cref="NotSupportedException">No provider is registered for the configured platform.</exception>
    public AppDeploymentOutputs Deploy(AppInfrastructureConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!_providers.TryGetValue(config.Provider, out IInfrastructureProvider? provider))
        {
            throw new NotSupportedException(
                $"No infrastructure provider is registered for '{config.Provider}'.");
        }

        return provider.Deploy(config);
    }
}
