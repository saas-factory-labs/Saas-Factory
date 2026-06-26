using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using Pulumi;

namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare;

/// <summary>
/// Provisions an AppBlueprint application's infrastructure on Cloudflare (the primary target).
/// Split across partial files by concern: compute (this step), storage, and database/Hyperdrive.
/// </summary>
public sealed partial class CloudflareProvider : ProviderBase
{
    /// <inheritdoc />
    public override CloudProvider Provider => CloudProvider.Cloudflare;

    /// <inheritdoc />
    public override AppDeploymentOutputs Deploy(AppInfrastructureConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var outputs = new AppDeploymentOutputs();
        string accountId = ResolveAccountId();

        ProvisionCompute(config, accountId, outputs);
        ProvisionStorage(config, accountId, outputs);
        ProvisionDatabase(config, accountId, outputs);

        return outputs;
    }

    /// <summary>
    /// Resolves the Cloudflare account id required by every account-scoped resource, from Pulumi
    /// config (<c>cloudflare:accountId</c>) or the <c>CLOUDFLARE_ACCOUNT_ID</c> environment variable.
    /// </summary>
    private static string ResolveAccountId()
    {
        string? accountId = new Config("cloudflare").Get("accountId");
        if (string.IsNullOrWhiteSpace(accountId))
        {
            accountId = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID");
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new InvalidOperationException(
                "Cloudflare account id not found. Set 'cloudflare:accountId' in Pulumi config or " +
                "the CLOUDFLARE_ACCOUNT_ID environment variable.");
        }

        return accountId;
    }
}
