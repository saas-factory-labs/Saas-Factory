using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;

namespace AppBlueprint.Infrastructure.Payments.Services.Webhooks;

/// <summary>
/// Service for handling Stripe webhook events.
/// </summary>
public sealed class StripeWebhookService
{
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly string? _webhookSecret;

    public StripeWebhookService(ILogger<StripeWebhookService> logger, string? webhookSecret = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        _webhookSecret = webhookSecret;
    }

    /// <summary>
    /// Constructs and validates a Stripe webhook event from the raw payload and signature header.
    /// Returns null if signature verification fails.
    /// </summary>
    public Event? ConstructEvent(string payload, string signatureHeader)
    {
        if (string.IsNullOrEmpty(_webhookSecret))
        {
            _logger.LogWarning("Stripe webhook secret not configured — skipping signature verification");
            return EventUtility.ParseEvent(payload);
        }

        try
        {
            return EventUtility.ConstructEvent(payload, signatureHeader, _webhookSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook signature verification failed");
            return null;
        }
    }

    /// <summary>
    /// Processes a Stripe webhook event.
    /// </summary>
    public async Task ProcessWebhookEventAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stripeEvent);

        _logger.LogInformation("Processing Stripe webhook event {EventId} of type {EventType}",
            stripeEvent.Id, stripeEvent.Type);

        try
        {
            switch (stripeEvent.Type)
            {
                case "customer.created":
                    await HandleCustomerCreatedAsync(stripeEvent, cancellationToken);
                    break;
                case "customer.updated":
                    await HandleCustomerUpdatedAsync(stripeEvent, cancellationToken);
                    break;
                case "customer.deleted":
                    await HandleCustomerDeletedAsync(stripeEvent, cancellationToken);
                    break;
                case "customer.subscription.created":
                    await HandleSubscriptionCreatedAsync(stripeEvent, cancellationToken);
                    break;
                case "customer.subscription.updated":
                    await HandleSubscriptionUpdatedAsync(stripeEvent, cancellationToken);
                    break;
                case "customer.subscription.deleted":
                    await HandleSubscriptionDeletedAsync(stripeEvent, cancellationToken);
                    break;
                case "invoice.paid":
                    await HandleInvoicePaidAsync(stripeEvent, cancellationToken);
                    break;
                case "invoice.payment_failed":
                    await HandleInvoicePaymentFailedAsync(stripeEvent, cancellationToken);
                    break;
                case "checkout.session.completed":
                    await HandleCheckoutSessionCompletedAsync(stripeEvent, cancellationToken);
                    break;
                case "payment_intent.succeeded":
                    await HandlePaymentIntentSucceededAsync(stripeEvent, cancellationToken);
                    break;
                case "payment_intent.payment_failed":
                    await HandlePaymentIntentFailedAsync(stripeEvent, cancellationToken);
                    break;
                default:
                    _logger.LogWarning("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe webhook event {EventId} of type {EventType}",
                stripeEvent.Id, stripeEvent.Type);
            throw;
        }
    }

    private Task HandleCustomerCreatedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Customer? customer = stripeEvent.Data.Object as Customer;
        if (customer is null) return Task.CompletedTask;
        _logger.LogInformation("Customer created: {CustomerId}, Email: {Email}", customer.Id, customer.Email);
        return Task.CompletedTask;
    }

    private Task HandleCustomerUpdatedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Customer? customer = stripeEvent.Data.Object as Customer;
        if (customer is null) return Task.CompletedTask;
        _logger.LogInformation("Customer updated: {CustomerId}", customer.Id);
        return Task.CompletedTask;
    }

    private Task HandleCustomerDeletedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Customer? customer = stripeEvent.Data.Object as Customer;
        if (customer is null) return Task.CompletedTask;
        _logger.LogInformation("Customer deleted: {CustomerId}", customer.Id);
        return Task.CompletedTask;
    }

    private Task HandleSubscriptionCreatedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Subscription? subscription = stripeEvent.Data.Object as Subscription;
        if (subscription is null) return Task.CompletedTask;
        _logger.LogInformation("Subscription created: {SubscriptionId}, Status: {Status}", subscription.Id, subscription.Status);
        return Task.CompletedTask;
    }

    private Task HandleSubscriptionUpdatedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Subscription? subscription = stripeEvent.Data.Object as Subscription;
        if (subscription is null) return Task.CompletedTask;
        _logger.LogInformation("Subscription updated: {SubscriptionId}, Status: {Status}", subscription.Id, subscription.Status);
        return Task.CompletedTask;
    }

    private Task HandleSubscriptionDeletedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Subscription? subscription = stripeEvent.Data.Object as Subscription;
        if (subscription is null) return Task.CompletedTask;
        _logger.LogInformation("Subscription deleted: {SubscriptionId}", subscription.Id);
        return Task.CompletedTask;
    }

    private Task HandleInvoicePaidAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Invoice? invoice = stripeEvent.Data.Object as Invoice;
        if (invoice is null) return Task.CompletedTask;
        _logger.LogInformation("Invoice paid: {InvoiceId}, Amount: {Amount}", invoice.Id, invoice.AmountPaid);
        return Task.CompletedTask;
    }

    private Task HandleInvoicePaymentFailedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Invoice? invoice = stripeEvent.Data.Object as Invoice;
        if (invoice is null) return Task.CompletedTask;
        _logger.LogWarning("Invoice payment failed: {InvoiceId}, Customer: {CustomerId}", invoice.Id, invoice.CustomerId);
        return Task.CompletedTask;
    }

    private Task HandleCheckoutSessionCompletedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        Session? session = stripeEvent.Data.Object as Session;
        if (session is null) return Task.CompletedTask;
        _logger.LogInformation("Checkout session completed: {SessionId}", session.Id);
        return Task.CompletedTask;
    }

    private Task HandlePaymentIntentSucceededAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        PaymentIntent? paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent is null) return Task.CompletedTask;
        _logger.LogInformation("Payment intent succeeded: {PaymentIntentId}, Amount: {Amount}", paymentIntent.Id, paymentIntent.Amount);
        return Task.CompletedTask;
    }

    private Task HandlePaymentIntentFailedAsync(Event stripeEvent, CancellationToken cancellationToken)
    {
        PaymentIntent? paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        if (paymentIntent is null) return Task.CompletedTask;
        _logger.LogWarning("Payment intent failed: {PaymentIntentId}", paymentIntent.Id);
        return Task.CompletedTask;
    }
}
