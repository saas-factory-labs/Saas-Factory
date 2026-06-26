using AppBlueprint.Infrastructure.Core.Abstractions;
using AppBlueprint.Infrastructure.Core.Configuration;
using Pulumi.Cloudflare;
using Pulumi.Cloudflare.Inputs;

namespace AppBlueprint.Infrastructure.Core.Providers.Cloudflare;

public sealed partial class CloudflareProvider
{
    // Identifier of the default delivery variant created when an app opts into Cloudflare Images.
    private const string DefaultImageVariantId = "public";

    private static void ProvisionStorage(AppInfrastructureConfig config, string accountId, AppDeploymentOutputs outputs)
    {
        if (config.Storage is null)
        {
            return;
        }

        foreach (string bucketName in config.Storage.Buckets)
        {
            var bucket = new R2Bucket(ResourceName(config.AppId, bucketName), new R2BucketArgs
            {
                AccountId = accountId,
                Name = bucketName
            });

            outputs.Set($"storage:bucket:{bucketName}", bucket.Name);
        }

        if (config.Storage.EnableImages)
        {
            // Cloudflare Images is enabled at the account level (a subscription), so there is no
            // "enable" resource. The provisionable, app-usable artifact is a named delivery variant;
            // create a sensible default "public" variant the application can request.
            var variant = new ImageVariant(
                ResourceName(config.AppId, "images-" + DefaultImageVariantId),
                new ImageVariantArgs
                {
                    AccountId = accountId,
                    ImageVariantId = DefaultImageVariantId,
                    Options = new ImageVariantOptionsArgs
                    {
                        Fit = "scale-down",
                        Width = 1280,
                        Height = 1280,
                        Metadata = "none"
                    }
                });

            outputs.Set("storage:imageVariant", variant.ImageVariantId);
        }
    }
}
