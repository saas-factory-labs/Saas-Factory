using Amazon.Runtime;
using Amazon.S3;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Persistence.Repositories;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using AppBlueprint.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Storage.Extensions;

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

    /// <summary>
    /// Registers the generic S3-compatible object storage service.
    /// Works with Cloudflare R2, AWS S3, MinIO, or any S3-compatible provider.
    /// </summary>
    public static IServiceCollection AddObjectStorageService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string? endpoint = Environment.GetEnvironmentVariable("STORAGE_ENDPOINT")
                        ?? configuration["Storage:Endpoint"];

        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            services.Configure<ObjectStorageOptions>(options =>
            {
                options.Endpoint = endpoint;
                options.BucketName = Environment.GetEnvironmentVariable("STORAGE_BUCKET_NAME")
                                  ?? configuration["Storage:BucketName"]
                                  ?? string.Empty;
                options.AccessKey = Environment.GetEnvironmentVariable("STORAGE_ACCESS_KEY")
                                 ?? configuration["Storage:AccessKey"]
                                 ?? string.Empty;
                options.SecretKey = Environment.GetEnvironmentVariable("STORAGE_SECRET_KEY")
                                 ?? configuration["Storage:SecretKey"]
                                 ?? string.Empty;
            });

            services.AddSingleton<IObjectStorageService, ObjectStorageService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] S3-compatible object storage service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Object storage not configured (optional)");
        }

        return services;
    }

    /// <summary>
    /// Registers the secure S3-compatible document storage service (<see cref="IStorageService"/>).
    /// Performs PDF magic bytes validation, filename sanitization, and security header injection.
    /// Works with Cloudflare R2, AWS S3, MinIO, or any S3-compatible provider.
    /// Reads configuration from STORAGE_ENDPOINT / STORAGE_ACCESS_KEY / STORAGE_SECRET_KEY / STORAGE_BUCKET_NAME.
    /// </summary>
    public static IServiceCollection AddS3StorageService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string? endpoint = Environment.GetEnvironmentVariable("STORAGE_ENDPOINT")
                        ?? configuration["STORAGE_ENDPOINT"];

        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            services.AddSingleton<IStorageService, S3StorageService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] S3 secure document storage service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] S3 secure document storage not configured (optional)");
        }

        return services;
    }
}
