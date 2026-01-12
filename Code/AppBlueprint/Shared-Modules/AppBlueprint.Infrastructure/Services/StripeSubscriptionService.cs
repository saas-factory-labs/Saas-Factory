using AppBlueprint.Application.Interfaces;
using AppBlueprint.Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Implementation of Stripe payment and subscription service.
/// </summary>
public sealed class StripeSubscriptionService : IStripeService
{
    private readonly StripeOptions _options;
    private readonly ILogger<StripeSubscriptionService> _logger;

    public StripeSubscriptionService(
        IOptions<StripeOptions> options,
        ILogger<StripeSubscriptionService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        
        _options = options.Value;
        _logger = logger;
        StripeConfiguration.ApiKey = _options.ApiKey;
    }

    /// <inheritdoc/>
    public async Task<Customer> CreateCustomerAsync(
        string email, 
        string? paymentMethodId = null, 
        string? name = null, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        
        var customerOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };

        if (!string.IsNullOrWhiteSpace(paymentMethodId))
        {
            customerOptions.PaymentMethod = paymentMethodId;
            customerOptions.InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId
            };
        }

        var customerService = new CustomerService();
        Customer customer = await customerService.CreateAsync(customerOptions, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Created Stripe customer {CustomerId} for email {Email}", customer.Id, email);
        return customer;
    }

    /// <inheritdoc/>
    public async Task<Customer?> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        
        try
        {
            var customerService = new CustomerService();
            return await customerService.GetAsync(customerId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex) when (ex.StripeError.Type == "invalid_request_error")
        {
            _logger.LogWarning("Stripe customer {CustomerId} not found", customerId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<Customer> UpdateCustomerAsync(
        string customerId, 
        string? email = null, 
        string? name = null, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        
        var customerOptions = new CustomerUpdateOptions();
        
        if (!string.IsNullOrWhiteSpace(email))
            customerOptions.Email = email;
            
        if (!string.IsNullOrWhiteSpace(name))
            customerOptions.Name = name;

        var customerService = new CustomerService();
        Customer customer = await customerService.UpdateAsync(customerId, customerOptions, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Updated Stripe customer {CustomerId}", customerId);
        return customer;
    }

    /// <inheritdoc/>
    public async Task<Customer> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        
        var customerService = new CustomerService();
        Customer customer = await customerService.DeleteAsync(customerId, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Deleted Stripe customer {CustomerId}", customerId);
        return customer;
    }

    /// <inheritdoc/>
    public async Task<Subscription> CreateSubscriptionAsync(
        string customerId, 
        string priceId, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        ArgumentNullException.ThrowIfNull(priceId);
        
        var subscriptionOptions = new SubscriptionCreateOptions
        {
            Customer = customerId,
            Items = new List<SubscriptionItemOptions>
            {
                new()
                {
                    Price = priceId
                }
            },
            Expand = new List<string> { "latest_invoice.payment_intent" },
            PaymentBehavior = "default_incomplete"
        };

        var subscriptionService = new SubscriptionService();
        Subscription subscription = await subscriptionService.CreateAsync(subscriptionOptions, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Created Stripe subscription {SubscriptionId} for customer {CustomerId}", 
            subscription.Id, customerId);
        return subscription;
    }

    /// <inheritdoc/>
    public async Task<Subscription?> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subscriptionId);
        
        try
        {
            var subscriptionService = new SubscriptionService();
            return await subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);
        }
        catch (StripeException ex) when (ex.StripeError.Type == "invalid_request_error")
        {
            _logger.LogWarning("Stripe subscription {SubscriptionId} not found", subscriptionId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<StripeList<Subscription>> ListSubscriptionsAsync(
        string customerId, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customerId);
        
        var subscriptionService = new SubscriptionService();
        var options = new SubscriptionListOptions
        {
            Customer = customerId
        };
        
        return await subscriptionService.ListAsync(options, cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Subscription> UpdateSubscriptionAsync(
        string subscriptionId, 
        string? priceId = null, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subscriptionId);
        
        var subscriptionOptions = new SubscriptionUpdateOptions();

        if (!string.IsNullOrWhiteSpace(priceId))
        {
            subscriptionOptions.Items = new List<SubscriptionItemOptions>
            {
                new() { Price = priceId }
            };
        }

        var subscriptionService = new SubscriptionService();
        Subscription subscription = await subscriptionService.UpdateAsync(
            subscriptionId, 
            subscriptionOptions, 
            cancellationToken: cancellationToken);
        
        _logger.LogInformation("Updated Stripe subscription {SubscriptionId}", subscriptionId);
        return subscription;
    }

    /// <inheritdoc/>
    public async Task<Subscription> CancelSubscriptionAsync(
        string subscriptionId, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(subscriptionId);
        
        var subscriptionService = new SubscriptionService();
        Subscription subscription = await subscriptionService.CancelAsync(subscriptionId, cancellationToken: cancellationToken);
        
        _logger.LogInformation("Canceled Stripe subscription {SubscriptionId}", subscriptionId);
        return subscription;
    }

    /// <inheritdoc/>
    public async Task<PaymentIntent> CreatePaymentIntentAsync(
        long amount, 
        string currency, 
        string? customerId = null, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(currency);
        
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        var paymentIntentOptions = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            }
        };

        if (!string.IsNullOrWhiteSpace(customerId))
        {
            paymentIntentOptions.Customer = customerId;
        }

        var paymentIntentService = new PaymentIntentService();
        PaymentIntent paymentIntent = await paymentIntentService.CreateAsync(
            paymentIntentOptions, 
            cancellationToken: cancellationToken);
        
        _logger.LogInformation("Created payment intent {PaymentIntentId} for amount {Amount} {Currency}", 
            paymentIntent.Id, amount, currency);
        return paymentIntent;
    }

    /// <inheritdoc/>
    public Event ConstructWebhookEvent(string json, string stripeSignature)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentNullException.ThrowIfNull(stripeSignature);
        
        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            throw new InvalidOperationException("Stripe WebhookSecret is not configured");
        }

        return EventUtility.ConstructEvent(json, stripeSignature, _options.WebhookSecret);
    }
}
