namespace AppBlueprint.Infrastructure.Core.Configuration;

/// <summary>
/// Marks that an application uses a Postgres database. The database itself (Neon serverless Postgres)
/// is provisioned and managed manually in the Neon portal — the infrastructure engine creates no
/// database resources. The connection string is supplied at deploy time as a Pulumi secret
/// (<c>neon:connectionString</c>) or environment variable and is never stored in <c>infra.json</c>.
/// Presence of this section is what enables fronting the database with Cloudflare Hyperdrive.
/// </summary>
public sealed record DatabaseArgs
{
    /// <summary>Optional human-readable label for the database. Documentation only; not provisioned.</summary>
    public string? Name { get; init; }

    /// <summary>
    /// When true (default), the database is fronted by a Cloudflare Hyperdrive config so Workers get
    /// pooled, low-latency connections. Set false to connect directly without Hyperdrive.
    /// </summary>
    public bool UseHyperdrive { get; init; } = true;
}
