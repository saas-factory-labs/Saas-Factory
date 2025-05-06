# External Integrations

When implementing integration to external services, follow these rules very carefully.

## Structure

Integration to external services should be implemented as a client in a file located at `/Code/AppBlueprint/Shared-Modules/AppBlueprint.Infrastructure/Services[ServiceName]/[NameService].cs`.

## Implementation

1. Create a client class with a clear purpose and name.
2. Implement proper error handling and logging.
3. Return appropriate types (null, optional, or Result types) rather than throwing exceptions.
4. Use typed clients with HttpClient injection (via AddHttpClient<T>) for HTTP-based integrations.
5. Consider timeouts, retry policies, and circuit breakers for resilience.
6. Create DTOs for request and response data when needed (but don't postfix with `Dto`).
7. Keep implementation in one file.

## Example 1 - Stripe Subscription Integration

```csharp
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace AppBlueprint.Infrastructure.Services;

public class StripeSubscriptionService
{
    private readonly string _stripeApiKey;


    public StripeSubscriptionService(IConfiguration configuration)
    {
        _stripeApiKey = configuration.GetConnectionString("StripeApiKey");

        StripeConfiguration.ApiKey = _stripeApiKey;
    }

    public CustomerEntity CreateCustomer(string email, string paymentMethodId)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Email = email,
            PaymentMethod = paymentMethodId,
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId
            }
        };
        var customerService = new CustomerService();
        return null;
        //return customerService.Create(customerOptions);
    }

    public Subscription CreateSubscription(string customerId, string priceId)
    {
        // Create a subscription
        var subscriptionOptions = new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions>
            {
                new()
                {
                    Price = priceId // Price ID from your Stripe Dashboard
                }
            },
            Expand = new List<string> { "latest_invoice.payment_intent" }
        };
        var subscriptionService = new SubscriptionService();
        return subscriptionService.Create(subscriptionOptions);
    }
}

```

## Key Implementation Details

1. Constructor Injection: Uses constructor injection with primary constructor syntax for dependencies.
2. Typed Client Pattern: Uses the HttpClient directly via constructor injection.
3. Error Handling: Properly handles HTTP status codes and avoid throwing exceptions.
4. Logging: Logs information and errors with structured logging.
5. Return Type: Returns null when the resource is not found or an error occurs.
6. Cancellation Support: Accepts and passes through a CancellationToken.

## Registration

Register the client in the DI container using the typed client pattern:

```csharp
services.AddHttpClient<StripeSubscriptionService>(client =>
{
    client.BaseAddress = new Uri("https://api.stripe.com/");
    client.Timeout = TimeSpan.FromSeconds(5);
});
```

For additional resilience, you can add Polly policies (requires `Microsoft.Extensions.Http.Polly` package):

```csharp
services.AddHttpClient<StripeSubscriptionService>(client =>
{
    client.BaseAddress = new Uri("https://api.stripe.com/");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(
    new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1) }
));
```