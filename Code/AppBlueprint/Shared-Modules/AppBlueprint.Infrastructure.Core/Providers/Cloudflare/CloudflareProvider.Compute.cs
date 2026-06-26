using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using Pulumi.Cloudflare;

namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare;

public sealed partial class CloudflareProvider
{
    // Cloudflare's container product is not yet exposed by the Pulumi provider, so a Workers script
    // is the "equivalent container script API". Infrastructure owns the script resource (its account
    // binding, name and routing); the worker's actual code and secret bindings stay owned by
    // wrangler/CI and the Cloudflare dashboard — preserved across deployments by the ignore set below.

    // Property paths ignored so portal-/wrangler-managed values are never reverted on the next
    // `pulumi up`. In Cloudflare provider v6 secrets live inside `bindings` and code in `content`.
    private static readonly string[] WorkerSecretAndCodeProperties = ["content", "contentSha256", "bindings"];

    // Minimal module installed only on first create; real code is published by wrangler/CI and is
    // kept across deployments because "content" is in WorkerSecretAndCodeProperties.
    private const string PlaceholderModule =
        "export default { async fetch() { return new Response('AppBlueprint: awaiting first deploy'); } };";

    private static void ProvisionCompute(AppInfrastructureConfig config, string accountId, AppDeploymentOutputs outputs)
    {
        foreach (ComputeArgs workload in config.Compute)
        {
            string resourceName = ResourceName(config.AppId, workload.Name);

            var script = new WorkersScript(resourceName, new WorkersScriptArgs
            {
                AccountId = accountId,
                ScriptName = resourceName,
                Content = PlaceholderModule,
                MainModule = "worker.js",
                CompatibilityDate = "2024-09-23"
            }, IgnorePortalManagedSecrets(WorkerSecretAndCodeProperties));

            if (workload.IsPublic)
            {
                outputs.SetEndpoint(workload.Name, script.ScriptName);
            }
        }
    }
}
