using AppBlueprint.Application.Interfaces.Storage;
using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Storage.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Storage.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint object storage services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint storage services including Cloudflare R2 (S3-compatible) object storage.
    /// Services are only registered if their configuration is present.
    /// </summary>
    public static IServiceCollection AddAppBlueprintStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        IServiceProvider tempProvider = services.BuildServiceProvider();
        CloudflareR2Options? r2Options = tempProvider.GetService<IOptions<CloudflareR2Options>>()?.Value;

        if (r2Options is not null &&
            !string.IsNullOrWhiteSpace(r2Options.AccessKeyId) &&
            !string.IsNullOrWhiteSpace(r2Options.SecretAccessKey) &&
            !string.IsNullOrWhiteSpace(r2Options.EndpointUrl) &&
            !string.IsNullOrWhiteSpace(r2Options.BucketName))
        {
            services.AddScoped<IFileStorageService, R2FileStorageService>();
            Console.WriteLine("[AppBlueprint.Infrastructure.Storage] Cloudflare R2 storage service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure.Storage] Cloudflare R2 not configured (optional)");
        }

        return services;
    }
}
