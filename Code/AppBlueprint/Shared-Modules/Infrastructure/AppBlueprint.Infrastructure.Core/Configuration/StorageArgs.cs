namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// Object storage settings. The primary implementation provisions Cloudflare R2 (S3-compatible)
/// buckets and optionally enables Cloudflare Images. Omit this section if the app stores no blobs.
/// </summary>
public sealed record StorageArgs
{
    /// <summary>Names of R2 (S3-compatible) buckets to provision for the application.</summary>
    public IReadOnlyList<string> Buckets { get; init; } = [];

    /// <summary>When true, enables Cloudflare Images for the application.</summary>
    public bool EnableImages { get; init; }
}
