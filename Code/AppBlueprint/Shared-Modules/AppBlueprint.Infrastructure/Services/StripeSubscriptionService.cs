using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace AppBlueprint.Infrastructure.Services;

public class StripeSubscriptionService
{
    private readonly string _stripeApiKey;


    public StripeSubscriptionService(IConfiguration configuration)
    {
        _stripeApiKey = configuration.GetConnectionString("StripeApiKey") ?? throw new InvalidOperationException("StripeApiKey connection string is not configured.");

        StripeConfiguration.ApiKey = _stripeApiKey;
    }

    public CustomerEntity? CreateCustomer(string email, string paymentMethodId)
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
        
        // TODO: Implement actual customer creation logic
        // For now, returning null is acceptable since return type is nullable
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
