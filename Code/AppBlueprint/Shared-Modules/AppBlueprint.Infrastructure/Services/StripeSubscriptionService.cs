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

    public async Task<CustomerEntity?> CreateCustomerAsync(string email, string paymentMethodId)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        
        if (string.IsNullOrEmpty(paymentMethodId))
            throw new ArgumentException("Payment method ID cannot be null or empty", nameof(paymentMethodId));

        try
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
            var stripeCustomer = await customerService.CreateAsync(customerOptions);
            
            // Map Stripe customer to our entity
            return new CustomerEntity
            {
                Id = stripeCustomer.Id,
                Name = stripeCustomer.Name ?? email,
                Email = stripeCustomer.Email ?? email,
                PhoneNumber = stripeCustomer.Phone ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (StripeException ex)
        {
            // Log the error but don't expose Stripe details
            throw new InvalidOperationException($"Failed to create customer: {ex.Message}", ex);
        }
    }

    public async Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId)
    {
        if (string.IsNullOrEmpty(customerId))
            throw new ArgumentException("Customer ID cannot be null or empty", nameof(customerId));
            
        if (string.IsNullOrEmpty(priceId))
            throw new ArgumentException("Price ID cannot be null or empty", nameof(priceId));

        try
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
            return await subscriptionService.CreateAsync(subscriptionOptions);
        }
        catch (StripeException ex)
        {
            throw new InvalidOperationException($"Failed to create subscription: {ex.Message}", ex);
        }
    }

    public async Task<Subscription> GetSubscriptionAsync(string subscriptionId)
    {
        if (string.IsNullOrEmpty(subscriptionId))
            throw new ArgumentException("Subscription ID cannot be null or empty", nameof(subscriptionId));

        try
        {
            var subscriptionService = new SubscriptionService();
            return await subscriptionService.GetAsync(subscriptionId);
        }
        catch (StripeException ex)
        {
            throw new InvalidOperationException($"Failed to retrieve subscription: {ex.Message}", ex);
        }
    }

    public async Task<Subscription> CancelSubscriptionAsync(string subscriptionId)
    {
        if (string.IsNullOrEmpty(subscriptionId))
            throw new ArgumentException("Subscription ID cannot be null or empty", nameof(subscriptionId));

        try
        {
            var subscriptionService = new SubscriptionService();
            return await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions());
        }
        catch (StripeException ex)
        {
            throw new InvalidOperationException($"Failed to cancel subscription: {ex.Message}", ex);
        }
    }

    public async Task<StripeList<Subscription>> GetCustomerSubscriptionsAsync(string customerId)
    {
        if (string.IsNullOrEmpty(customerId))
            throw new ArgumentException("Customer ID cannot be null or empty", nameof(customerId));

        try
        {
            var subscriptionService = new SubscriptionService();
            var options = new SubscriptionListOptions
            {
                Customer = customerId,
                Status = "all"
            };
            return await subscriptionService.ListAsync(options);
        }
        catch (StripeException ex)
        {
            throw new InvalidOperationException($"Failed to retrieve customer subscriptions: {ex.Message}", ex);
        }
    }
}
