using AppBlueprint.Infrastructure.Core.Configuration;

namespace AppBlueprint.Infrastructure.Core.Abstractions;

/// <summary>
/// A cloud-specific implementation that knows how to provision the resources described by an
/// <see cref="AppInfrastructureConfig"/>. One implementation exists per <see cref="CloudProvider"/>;
/// <see cref="AppBlueprint.Infrastructure.Core"/>'s stack factory selects the right one at runtime.
/// </summary>
public interface IInfrastructureProvider
{
    /// <summary>The cloud platform this provider targets. Must match the config's selected provider.</summary>
    CloudProvider Provider { get; }

    /// <summary>
    /// Provisions all resources described by <paramref name="config"/> by constructing Pulumi
    /// resources as a side effect, and returns the resulting stack outputs. Invoked from inside a
    /// Pulumi program (the Automation API inline program), never on its own.
    /// </summary>
    /// <param name="config">The application's infrastructure contract, loaded from <c>infra.json</c>.</param>
    /// <returns>The deployment's outputs (endpoints, connection strings, bucket names).</returns>
    AppDeploymentOutputs Deploy(AppInfrastructureConfig config);
}
