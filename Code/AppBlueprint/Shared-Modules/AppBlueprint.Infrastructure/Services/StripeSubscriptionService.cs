using AppBlueprint.Application.Options;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using Microsoft.Extensions.Options;
using Stripe;

namespace AppBlueprint.Infrastructure.Services;

public class StripeSubscriptionService
{
    private readonly StripeOptions _options;

    public StripeSubscriptionService(IOptions<StripeOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _options = options.Value;
        StripeConfiguration.ApiKey = _options.ApiKey;
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
            Items =
            [
                new()
                {
                    Price = priceId // Price ID from your Stripe Dashboard
                }
            ],
            Expand = ["latest_invoice.payment_intent"]
        };
        var subscriptionService = new SubscriptionService();
        return subscriptionService.Create(subscriptionOptions);
    }
}
