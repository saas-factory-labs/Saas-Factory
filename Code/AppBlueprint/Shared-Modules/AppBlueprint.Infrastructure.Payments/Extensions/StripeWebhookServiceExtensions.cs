using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Interfaces.Repositories;
using AppBlueprint.Infrastructure.Repositories;
using AppBlueprint.Infrastructure.Services.Webhooks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering Stripe webhook services.
/// </summary>
public static class StripeWebhookServiceExtensions
{
    /// <summary>
    /// Registers the Stripe webhook service and its dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddStripeWebhookService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Register repository
        services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();

        // Get webhook secret from configuration
        // Supports multiple naming conventions:
        // 1. STRIPE_WEBHOOK_SECRET (recommended)
        // 2. Stripe:WebhookSecret (legacy dotnet format)
        string? webhookSecret = configuration["STRIPE_WEBHOOK_SECRET"]
                                ?? configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrEmpty(webhookSecret))
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] WARNING: Stripe webhook secret not configured. Signature verification will be skipped.");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Stripe webhook service registered with signature verification");
        }

        // Register webhook service with the secret
        services.AddScoped<IStripeWebhookService>(sp =>
        {
            var repository = sp.GetRequiredService<IWebhookEventRepository>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<StripeWebhookService>>();
            return new StripeWebhookService(repository, logger, webhookSecret);
        });

        return services;
    }

    /// <summary>
    /// Registers the Stripe webhook service with an explicitly provided webhook secret.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="webhookSecret">The Stripe webhook endpoint secret.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddStripeWebhookService(
        this IServiceCollection services,
        string webhookSecret)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(webhookSecret);

        // Register repository
        services.AddScoped<IWebhookEventRepository, WebhookEventRepository>();

        // Register webhook service with the provided secret
        services.AddScoped<IStripeWebhookService>(sp =>
        {
            var repository = sp.GetRequiredService<IWebhookEventRepository>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<StripeWebhookService>>();
            return new StripeWebhookService(repository, logger, webhookSecret);
        });

        Console.WriteLine("[AppBlueprint.Infrastructure] Stripe webhook service registered");

        return services;
    }
}
