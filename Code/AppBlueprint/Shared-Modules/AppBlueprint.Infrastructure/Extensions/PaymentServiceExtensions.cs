using Microsoft.Extensions.DependencyInjection;
using AppBlueprint.Infrastructure.Services;

namespace AppBlueprint.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering payment services in dependency injection container.
/// </summary>
public static class PaymentServiceExtensions
{
    /// <summary>
    /// Registers payment-related services including Stripe integration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<StripeSubscriptionService>();
        
        return services;
    }
}