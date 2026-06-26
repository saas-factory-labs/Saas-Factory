namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare.Wrangler;

/// <summary>
/// The subset of an existing <c>wrangler.toml</c> that the infrastructure engine bridges into a
/// deployment, so legacy Cloudflare setups (worker name, compatibility settings, and existing R2 /
/// Hyperdrive / var bindings) are respected rather than broken.
/// </summary>
public sealed record WranglerConfig
{
    /// <summary>The worker <c>name</c>, if declared.</summary>
    public string? Name { get; init; }

    /// <summary>The worker entry module (<c>main</c>), if declared.</summary>
    public string? Main { get; init; }

    /// <summary>The Cloudflare <c>account_id</c>, if declared.</summary>
    public string? AccountId { get; init; }

    /// <summary>The <c>compatibility_date</c>, if declared.</summary>
    public string? CompatibilityDate { get; init; }

    /// <summary>The <c>compatibility_flags</c> array.</summary>
    public IReadOnlyList<string> CompatibilityFlags { get; init; } = [];

    /// <summary>Existing R2 bucket bindings (<c>[[r2_buckets]]</c>).</summary>
    public IReadOnlyList<WranglerR2Binding> R2Buckets { get; init; } = [];

    /// <summary>Existing Hyperdrive bindings (<c>[[hyperdrive]]</c>).</summary>
    public IReadOnlyList<WranglerHyperdriveBinding> HyperdriveBindings { get; init; } = [];

    /// <summary>Plain-text variables declared under <c>[vars]</c>.</summary>
    public IReadOnlyDictionary<string, string> Vars { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}

/// <summary>An R2 bucket binding from <c>wrangler.toml</c>.</summary>
public sealed record WranglerR2Binding(string Binding, string BucketName);

/// <summary>A Hyperdrive binding from <c>wrangler.toml</c>.</summary>
public sealed record WranglerHyperdriveBinding(string Binding, string Id);
