using Amazon.Runtime;
using Amazon.S3;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint file storage services.
/// </summary>
public static class StorageServiceCollectionExtensions
{
    /// <summary>
    /// Registers Cloudflare R2 object storage service if credentials are configured.
    /// Uses CloudflareR2Options from IOptions pattern.
    /// </summary>
    public static IServiceCollection AddCloudflareR2Service(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Get R2 options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        CloudflareR2Options? r2Options = tempProvider.GetService<IOptions<CloudflareR2Options>>()?.Value;

        if (r2Options is not null &&
            !string.IsNullOrWhiteSpace(r2Options.AccessKeyId) &&
            !string.IsNullOrWhiteSpace(r2Options.SecretAccessKey) &&
            !string.IsNullOrWhiteSpace(r2Options.EndpointUrl) &&
            (!string.IsNullOrWhiteSpace(r2Options.PrivateBucketName) || !string.IsNullOrWhiteSpace(r2Options.PublicBucketName)))
        {
            services.AddSingleton<IAmazonS3>(sp =>
            {
                CloudflareR2Options options = sp.GetRequiredService<IOptions<CloudflareR2Options>>().Value;
                var credentials = new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey);
                return new AmazonS3Client(credentials, new AmazonS3Config
                {
                    ServiceURL = options.EndpointUrl,
                    ForcePathStyle = true // Required for R2 compatibility
                });
            });

            services.AddSingleton<ObjectStorageService>();

            // Register new file storage services
            services.AddScoped<IFileStorageService, R2FileStorageService>();
            services.AddScoped<IFileValidationService, FileValidationService>();
            services.AddScoped<IFileMetadataRepository, FileMetadataRepository>();

            Console.WriteLine("[AppBlueprint.Infrastructure] Cloudflare R2 storage service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Cloudflare R2 not configured (optional)");
        }

        return services;
    }
}
