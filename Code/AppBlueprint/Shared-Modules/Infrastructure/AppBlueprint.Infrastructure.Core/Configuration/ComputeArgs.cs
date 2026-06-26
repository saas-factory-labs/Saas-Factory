namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// Describes a single containerised workload that the infrastructure engine provisions —
/// for example the Blazor web frontend, the API service, or the YARP gateway.
/// </summary>
public sealed record ComputeArgs
{
    /// <summary>Logical name of the workload, unique within the application (e.g. "web", "api", "gateway").</summary>
    public required string Name { get; init; }

    /// <summary>Fully-qualified container image reference, including registry and tag.</summary>
    public required string Image { get; init; }

    /// <summary>The container port the workload listens on. Defaults to 8080.</summary>
    public int Port { get; init; } = 8080;

    /// <summary>Number of running instances/replicas. Defaults to 1.</summary>
    public int InstanceCount { get; init; } = 1;

    /// <summary>
    /// When true, the workload is publicly routable (mapped to the custom domain or a generated
    /// hostname). Internal-only workloads such as background workers should set this to false.
    /// </summary>
    public bool IsPublic { get; init; } = true;
}
