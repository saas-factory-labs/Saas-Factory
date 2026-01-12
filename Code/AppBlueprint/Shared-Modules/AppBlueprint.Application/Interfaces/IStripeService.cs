using Stripe;

namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Interface for Stripe payment and subscription operations.
/// </summary>
public interface IStripeService
{
    /// <summary>
    /// Creates a new Stripe customer.
    /// </summary>
    /// <param name="email">Customer email address.</param>
    /// <param name="paymentMethodId">Stripe payment method ID.</param>
    /// <param name="name">Customer name (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created Stripe customer.</returns>
    Task<Customer> CreateCustomerAsync(string email, string? paymentMethodId = null, string? name = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a Stripe customer by ID.
    /// </summary>
    /// <param name="customerId">Stripe customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stripe customer or null if not found.</returns>
    Task<Customer?> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates a Stripe customer.
    /// </summary>
    /// <param name="customerId">Stripe customer ID.</param>
    /// <param name="email">New email (optional).</param>
    /// <param name="name">New name (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated Stripe customer.</returns>
    Task<Customer> UpdateCustomerAsync(string customerId, string? email = null, string? name = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a Stripe customer.
    /// </summary>
    /// <param name="customerId">Stripe customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Customer> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new subscription for a customer.
    /// </summary>
    /// <param name="customerId">Stripe customer ID.</param>
    /// <param name="priceId">Stripe price ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created subscription.</returns>
    Task<Subscription> CreateSubscriptionAsync(string customerId, string priceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a subscription by ID.
    /// </summary>
    /// <param name="subscriptionId">Stripe subscription ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Subscription or null if not found.</returns>
    Task<Subscription?> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all subscriptions for a customer.
    /// </summary>
    /// <param name="customerId">Stripe customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of subscriptions.</returns>
    Task<StripeList<Subscription>> ListSubscriptionsAsync(string customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates a subscription.
    /// </summary>
    /// <param name="subscriptionId">Stripe subscription ID.</param>
    /// <param name="priceId">New price ID (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated subscription.</returns>
    Task<Subscription> UpdateSubscriptionAsync(string subscriptionId, string? priceId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    /// <param name="subscriptionId">Stripe subscription ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Canceled subscription.</returns>
    Task<Subscription> CancelSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a payment intent for one-time payments.
    /// </summary>
    /// <param name="amount">Amount in cents.</param>
    /// <param name="currency">Currency code (e.g., "usd").</param>
    /// <param name="customerId">Stripe customer ID (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created payment intent.</returns>
    Task<PaymentIntent> CreatePaymentIntentAsync(long amount, string currency, string? customerId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Constructs a Stripe event from webhook request data.
    /// </summary>
    /// <param name="json">Raw JSON payload from webhook.</param>
    /// <param name="stripeSignature">Stripe-Signature header value.</param>
    /// <returns>Constructed Stripe event.</returns>
    Event ConstructWebhookEvent(string json, string stripeSignature);
}
