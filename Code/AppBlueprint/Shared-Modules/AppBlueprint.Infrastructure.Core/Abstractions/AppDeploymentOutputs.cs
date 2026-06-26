namespace AppBlueprint.Infrastructure.Core.Abstractions;

/// <summary>
/// Collects the stack outputs a provider produces while provisioning an application — typically
/// public endpoint URLs, a database connection string, and storage bucket names.
/// </summary>
/// <remarks>
/// Values are stored as <see cref="object"/> so this type stays free of any Pulumi dependency: at
/// runtime a value is usually a Pulumi <c>Output&lt;string&gt;</c> (often a secret), but a plain
/// resolved <see cref="string"/> is equally valid. <see cref="AppBlueprint.Infrastructure.Core"/>
/// hands <see cref="ToExportDictionary"/> back to the Pulumi program for export.
/// </remarks>
public sealed class AppDeploymentOutputs
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    /// <summary>Well-known output key for a workload's public URL, suffixed with the workload name.</summary>
    public const string EndpointKeyPrefix = "endpoint:";

    /// <summary>Well-known output key for the provisioned database connection string (secret).</summary>
    public const string DatabaseConnectionStringKey = "database:connectionString";

    /// <summary>The accumulated outputs. Keys are ordinal and unique.</summary>
    public IReadOnlyDictionary<string, object?> Values => _values;

    /// <summary>Records (or overwrites) a named output value and returns this instance for chaining.</summary>
    /// <param name="key">Non-empty, unique output key.</param>
    /// <param name="value">The output value (commonly a Pulumi <c>Output&lt;string&gt;</c>).</param>
    public AppDeploymentOutputs Set(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _values[key] = value;
        return this;
    }

    /// <summary>Records a workload's public endpoint under the <see cref="EndpointKeyPrefix"/> convention.</summary>
    /// <param name="workloadName">Logical workload name (e.g. "web", "api").</param>
    /// <param name="url">The endpoint value (commonly a Pulumi <c>Output&lt;string&gt;</c>).</param>
    public AppDeploymentOutputs SetEndpoint(string workloadName, object? url)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workloadName);
        return Set(EndpointKeyPrefix + workloadName, url);
    }

    /// <summary>Produces a fresh dictionary suitable for returning from a Pulumi inline program.</summary>
    public IDictionary<string, object?> ToExportDictionary() =>
        new Dictionary<string, object?>(_values, StringComparer.Ordinal);
}
