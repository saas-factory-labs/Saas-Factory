using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Payments.Services.Webhooks;
using AppBlueprint.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace AppBlueprint.Infrastructure.Payments.Extensions;

/// <summary>
/// Extension methods for registering AppBlueprint payment services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AppBlueprint payment services including Stripe client, subscription service, and webhook service.
    /// Services are only registered if their configuration is present.
    /// </summary>
    public static IServiceCollection AddAppBlueprintPayments(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        IServiceProvider tempProvider = services.BuildServiceProvider();
        StripeOptions? stripeOptions = tempProvider.GetService<IOptions<StripeOptions>>()?.Value;

        if (stripeOptions is not null && !string.IsNullOrWhiteSpace(stripeOptions.ApiKey))
        {
            StripeConfiguration.ApiKey = stripeOptions.ApiKey;
            services.AddSingleton<IStripeClient>(new StripeClient(stripeOptions.ApiKey));
            services.AddScoped<StripeSubscriptionService>();
            Console.WriteLine("[AppBlueprint.Infrastructure.Payments] Stripe client and subscription service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure.Payments] Stripe not configured (optional)");
        }

        string? webhookSecret = configuration["STRIPE_WEBHOOK_SECRET"]
                                ?? configuration["Stripe:WebhookSecret"];

        if (string.IsNullOrEmpty(webhookSecret))
        {
            Console.WriteLine("[AppBlueprint.Infrastructure.Payments] WARNING: Stripe webhook secret not configured. Signature verification will be skipped.");
        }

        services.AddScoped<StripeWebhookService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<StripeWebhookService>>();
            return new StripeWebhookService(logger, webhookSecret);
        });

        return services;
    }
}
