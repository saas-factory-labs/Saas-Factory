using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.Payments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AppBlueprint.Infrastructure.Payments.Extensions;

/// <summary>
/// Extension methods for registering Stripe payment services.
/// </summary>
public static class StripeServiceCollectionExtensions
{
    /// <summary>
    /// Registers Stripe payment service if API key is configured.
    /// Uses StripeOptions from IOptions pattern.
    /// </summary>
    public static IServiceCollection AddStripeService(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Get Stripe options to check if configured
        IServiceProvider tempProvider = services.BuildServiceProvider();
        StripeOptions? stripeOptions = tempProvider.GetService<IOptions<StripeOptions>>()?.Value;

        if (stripeOptions is not null && !string.IsNullOrWhiteSpace(stripeOptions.ApiKey))
        {
            services.AddScoped<StripeSubscriptionService>();
            Console.WriteLine("[AppBlueprint.Infrastructure] Stripe service registered");
        }
        else
        {
            Console.WriteLine("[AppBlueprint.Infrastructure] Stripe not configured (optional)");
        }

        return services;
    }
}
