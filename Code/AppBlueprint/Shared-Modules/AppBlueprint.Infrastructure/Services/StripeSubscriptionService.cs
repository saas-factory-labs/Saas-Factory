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
