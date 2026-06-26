using System.Text.Json.Serialization;

namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// The target cloud platform an AppBlueprint-based application is deployed to.
/// Serialized as its name (e.g. "Cloudflare") so <c>infra.json</c> stays human-readable.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<CloudProvider>))]
public enum CloudProvider
{
    /// <summary>Primary target. Workers for Containers, R2 object storage, Cloudflare Images.</summary>
    Cloudflare,

    /// <summary>Secondary target. Container services and managed Postgres on Railway.</summary>
    Railway,

    /// <summary>Tertiary target. Hetzner compute via Cloudfleet CFKE (managed Kubernetes).</summary>
    Hetzner
}
