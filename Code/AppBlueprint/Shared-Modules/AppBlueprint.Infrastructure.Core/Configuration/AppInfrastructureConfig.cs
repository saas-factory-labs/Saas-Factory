using System.Text.Json.Serialization;

namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// Root configuration contract for an AppBlueprint-based SaaS application's infrastructure.
/// A SaaS app declares only this document (as <c>infra.json</c>); all provisioning logic lives in the
/// central infrastructure engine driven by the AppBlueprint Developer CLI. This type is both the
/// deserialization target for <c>infra.json</c> and the source the CLI reflects over to generate
/// <c>app-infra-schema.json</c>.
/// </summary>
public sealed record AppInfrastructureConfig
{
    /// <summary>
    /// JSON Schema reference for IDE IntelliSense. Points at the generated <c>app-infra-schema.json</c>.
    /// </summary>
    [JsonPropertyName("$schema")]
    public string? Schema { get; init; }

    /// <summary>Globally-unique, stable identifier for the application (e.g. "dating", "property-rental").</summary>
    public required string AppId { get; init; }

    /// <summary>Target cloud platform for this application.</summary>
    public required CloudProvider Provider { get; init; }

    /// <summary>Optional custom domain the public workloads are served from.</summary>
    public string? CustomDomain { get; init; }

    /// <summary>Containerised workloads that make up the application.</summary>
    public IReadOnlyList<ComputeArgs> Compute { get; init; } = [];

    /// <summary>Optional database provisioning settings. Omit for stateless applications.</summary>
    public DatabaseArgs? Database { get; init; }

    /// <summary>Optional object storage settings. Omit if the app stores no blobs.</summary>
    public StorageArgs? Storage { get; init; }
}
