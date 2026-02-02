using AppBlueprint.Application.Interfaces;
using AppBlueprint.Domain.Entities.Webhooks;
using AppBlueprint.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;

namespace AppBlueprint.Infrastructure.Services.Webhooks;

/// <summary>
/// Service for processing Stripe webhook events.
/// Implements signature verification, idempotency checking, event storage, and business logic dispatch.
/// </summary>
public sealed class StripeWebhookService : IStripeWebhookService
{
    private readonly IWebhookEventRepository _webhookEventRepository;
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly string? _webhookSecret;
    private const int MaxRetryAttempts = 3;
    private const string Source = "stripe";

    public StripeWebhookService(
        IWebhookEventRepository webhookEventRepository,
        ILogger<StripeWebhookService> logger,
        string? webhookSecret = null)
    {
        ArgumentNullException.ThrowIfNull(webhookEventRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _webhookEventRepository = webhookEventRepository;
        _logger = logger;
        _webhookSecret = webhookSecret;
    }

    /// <inheritdoc />
    public async Task<WebhookProcessingResult> ProcessWebhookAsync(
        string payload,
        string signatureHeader,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(signatureHeader);

        try
        {
            // Step 1: Verify webhook signature
            Event stripeEvent = VerifySignature(payload, signatureHeader);

            _logger.LogInformation(
                "Received Stripe webhook event: {EventId}, Type: {EventType}",
                stripeEvent.Id,
                stripeEvent.Type
            );

            // Step 2: Check for duplicate events (idempotency)
            WebhookEventEntity? existingEvent = await _webhookEventRepository.GetByEventIdAsync(
                stripeEvent.Id,
                Source,
                cancellationToken
            );

            if (existingEvent is not null)
            {
                _logger.LogInformation(
                    "Duplicate webhook event detected: {EventId}. Skipping processing",
                    stripeEvent.Id
                );

                return WebhookProcessingResult.Successful(
                    stripeEvent.Id,
                    stripeEvent.Type,
                    wasDuplicate: true
                );
            }

            // Step 3: Extract tenant ID from event metadata (if available)
            string? tenantId = ExtractTenantIdFromEvent(stripeEvent);

            // Step 4: Store webhook event in database
            var webhookEvent = new WebhookEventEntity(
                stripeEvent.Id,
                stripeEvent.Type,
                Source,
                payload,
                tenantId
            );

            await _webhookEventRepository.AddAsync(webhookEvent, cancellationToken);

            // Step 5: Process the webhook event
            await ProcessEventAsync(stripeEvent, webhookEvent, cancellationToken);

            // Step 6: Mark as processed
            webhookEvent.MarkAsProcessed();
            await _webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);

            _logger.LogInformation(
                "Successfully processed Stripe webhook event: {EventId}",
                stripeEvent.Id
            );

            return WebhookProcessingResult.Successful(stripeEvent.Id, stripeEvent.Type);
        }
        catch (StripeException ex)
        {
            _logger.LogError(
                ex,
                "Stripe webhook signature verification failed: {Message}",
                ex.Message
            );

            return WebhookProcessingResult.Failed($"Signature verification failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing Stripe webhook: {Message}",
                ex.Message
            );

            return WebhookProcessingResult.Failed($"Processing error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<int> RetryFailedEventsAsync(int maxRetries = MaxRetryAttempts, CancellationToken cancellationToken = default)
    {
        IEnumerable<WebhookEventEntity> failedEvents = await _webhookEventRepository.GetFailedEventsForRetryAsync(maxRetries, cancellationToken);
        int retriedCount = 0;

        foreach (WebhookEventEntity webhookEvent in failedEvents)
        {
            try
            {
                _logger.LogInformation(
                    "Retrying failed webhook event: {EventId}, Attempt: {RetryCount}",
                    webhookEvent.EventId,
                    webhookEvent.RetryCount + 1
                );

                // Parse the stored payload back to Stripe event
                Event stripeEvent = EventUtility.ParseEvent(webhookEvent.Payload);

                // Retry processing
                await ProcessEventAsync(stripeEvent, webhookEvent, cancellationToken);

                // Mark as processed
                webhookEvent.MarkAsProcessed();
                await _webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);

                retriedCount++;

                _logger.LogInformation(
                    "Successfully retried webhook event: {EventId}",
                    webhookEvent.EventId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrying webhook event: {EventId}, Attempt: {RetryCount}",
                    webhookEvent.EventId,
                    webhookEvent.RetryCount
                );

                if (webhookEvent.RetryCount >= maxRetries)
                {
                    webhookEvent.MarkAsPermanentlyFailed($"Max retries exceeded: {ex.Message}");
                }
                else
                {
                    webhookEvent.MarkAsFailed($"Retry failed: {ex.Message}");
                }

                await _webhookEventRepository.UpdateAsync(webhookEvent, cancellationToken);
            }
        }

        return retriedCount;
    }

    /// <inheritdoc />
    public async Task<WebhookEventDetails?> GetWebhookEventByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        WebhookEventEntity? webhookEvent = await _webhookEventRepository.GetByIdAsync(id, cancellationToken);

        return webhookEvent is not null ? MapToDetails(webhookEvent) : null;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebhookEventDetails>> GetWebhookEventsByTenantIdAsync(
        string tenantId,
        int pageSize = 50,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(tenantId);

        IEnumerable<WebhookEventEntity> events = await _webhookEventRepository.GetByTenantIdAsync(
            tenantId,
            pageSize,
            pageNumber,
            cancellationToken
        );

        return events.Select(MapToDetails);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WebhookEventDetails>> GetRecentWebhookEventsAsync(int maxResults = 50, CancellationToken cancellationToken = default)
    {
        IEnumerable<WebhookEventEntity> events = await _webhookEventRepository.GetByStatusAsync(
            WebhookEventStatus.Processed,
            maxResults,
            cancellationToken
        );

        return events.Select(MapToDetails);
    }

    /// <summary>
    /// Verifies the Stripe webhook signature.
    /// </summary>
    private Event VerifySignature(string payload, string signatureHeader)
    {
        if (string.IsNullOrEmpty(_webhookSecret))
        {
            _logger.LogWarning("Webhook secret not configured. Skipping signature verification (NOT RECOMMENDED FOR PRODUCTION)");
            return EventUtility.ParseEvent(payload);
        }

        // Stripe.net automatically verifies the signature and throws StripeException if invalid
        return EventUtility.ConstructEvent(
            payload,
            signatureHeader,
            _webhookSecret,
            throwOnApiVersionMismatch: false // Allow API version differences
        );
    }

    /// <summary>
    /// Extracts tenant ID from Stripe event metadata.
    /// Stripe allows custom metadata on objects like customers, subscriptions, etc.
    /// </summary>
    private string? ExtractTenantIdFromEvent(Event stripeEvent)
    {
        try
        {
            // Try to extract tenant_id from metadata
            if (stripeEvent.Data?.Object is StripeEntity stripeEntity)
            {
                // Most Stripe objects have a Metadata property
                if (stripeEntity is IHasMetadata metadataEntity && metadataEntity.Metadata is not null)
                {
                    if (metadataEntity.Metadata.TryGetValue("tenant_id", out string? tenantId))
                    {
                        return tenantId;
                    }
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Error extracting tenant ID from Stripe event: {EventType}",
                stripeEvent.Type
            );
            return null;
        }
    }

    /// <summary>
    /// Processes the Stripe event and dispatches to appropriate handlers.
    /// </summary>
    private async Task ProcessEventAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        // Dispatch to event-specific handlers based on event type
        switch (stripeEvent.Type)
        {
            // Payment Intent events
            case "payment_intent.succeeded":
                await HandlePaymentIntentSucceededAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "payment_intent.payment_failed":
                await HandlePaymentIntentFailedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            // Customer events
            case "customer.created":
                await HandleCustomerCreatedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "customer.updated":
                await HandleCustomerUpdatedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "customer.deleted":
                await HandleCustomerDeletedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            // Subscription events
            case "customer.subscription.created":
                await HandleSubscriptionCreatedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "customer.subscription.updated":
                await HandleSubscriptionUpdatedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "customer.subscription.deleted":
                await HandleSubscriptionDeletedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            // Invoice events
            case "invoice.paid":
                await HandleInvoicePaidAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            case "invoice.payment_failed":
                await HandleInvoicePaymentFailedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            // Checkout events
            case "checkout.session.completed":
                await HandleCheckoutSessionCompletedAsync(stripeEvent, webhookEvent, cancellationToken);
                break;

            default:
                _logger.LogWarning(
                    "Unhandled Stripe webhook event type: {EventType}",
                    stripeEvent.Type
                );
                break;
        }
    }

    // Event-specific handler methods (to be implemented based on business requirements)

    private async Task HandlePaymentIntentSucceededAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        ArgumentNullException.ThrowIfNull(paymentIntent);

        _logger.LogInformation(
            "Payment succeeded: {PaymentIntentId}, Amount: {Amount}",
            paymentIntent.Id,
            paymentIntent.Amount
        );

        // TODO: Implement business logic (e.g., update order status, send receipt email)
        await Task.CompletedTask;
    }

    private async Task HandlePaymentIntentFailedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        ArgumentNullException.ThrowIfNull(paymentIntent);

        _logger.LogWarning(
            "Payment failed: {PaymentIntentId}, Reason: {FailureMessage}",
            paymentIntent.Id,
            paymentIntent.LastPaymentError?.Message ?? "Unknown"
        );

        // TODO: Implement business logic (e.g., notify user, update order status)
        await Task.CompletedTask;
    }

    private async Task HandleCustomerCreatedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var customer = stripeEvent.Data.Object as Customer;
        ArgumentNullException.ThrowIfNull(customer);

        _logger.LogInformation(
            "Customer created: {CustomerId}, Email: {Email}",
            customer.Id,
            customer.Email
        );

        // TODO: Implement business logic (e.g., sync customer to local database)
        await Task.CompletedTask;
    }

    private async Task HandleCustomerUpdatedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var customer = stripeEvent.Data.Object as Customer;
        ArgumentNullException.ThrowIfNull(customer);

        _logger.LogInformation(
            "Customer updated: {CustomerId}",
            customer.Id
        );

        // TODO: Implement business logic
        await Task.CompletedTask;
    }

    private async Task HandleCustomerDeletedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var customer = stripeEvent.Data.Object as Customer;
        ArgumentNullException.ThrowIfNull(customer);

        _logger.LogInformation(
            "Customer deleted: {CustomerId}",
            customer.Id
        );

        // TODO: Implement business logic
        await Task.CompletedTask;
    }

    private async Task HandleSubscriptionCreatedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        ArgumentNullException.ThrowIfNull(subscription);

        _logger.LogInformation(
            "Subscription created: {SubscriptionId}, CustomerId: {CustomerId}",
            subscription.Id,
            subscription.CustomerId
        );

        // TODO: Implement business logic (e.g., grant access to premium features)
        await Task.CompletedTask;
    }

    private async Task HandleSubscriptionUpdatedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        ArgumentNullException.ThrowIfNull(subscription);

        _logger.LogInformation(
            "Subscription updated: {SubscriptionId}, Status: {Status}",
            subscription.Id,
            subscription.Status
        );

        // TODO: Implement business logic (e.g., update subscription status)
        await Task.CompletedTask;
    }

    private async Task HandleSubscriptionDeletedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        ArgumentNullException.ThrowIfNull(subscription);

        _logger.LogInformation(
            "Subscription deleted: {SubscriptionId}",
            subscription.Id
        );

        // TODO: Implement business logic (e.g., revoke access to premium features)
        await Task.CompletedTask;
    }

    private async Task HandleInvoicePaidAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        ArgumentNullException.ThrowIfNull(invoice);

        _logger.LogInformation(
            "Invoice paid: {InvoiceId}, Amount: {Amount}",
            invoice.Id,
            invoice.AmountPaid
        );

        // TODO: Implement business logic (e.g., send receipt, update accounting system)
        await Task.CompletedTask;
    }

    private async Task HandleInvoicePaymentFailedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var invoice = stripeEvent.Data.Object as Invoice;
        ArgumentNullException.ThrowIfNull(invoice);

        _logger.LogWarning(
            "Invoice payment failed: {InvoiceId}",
            invoice.Id
        );

        // TODO: Implement business logic (e.g., notify user, suspend service)
        await Task.CompletedTask;
    }

    private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent, WebhookEventEntity webhookEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
        ArgumentNullException.ThrowIfNull(session);

        _logger.LogInformation(
            "Checkout session completed: {SessionId}, CustomerId: {CustomerId}",
            session.Id,
            session.CustomerId
        );

        // TODO: Implement business logic (e.g., fulfill order, send confirmation)
        await Task.CompletedTask;
    }

    /// <summary>
    /// Maps a WebhookEventEntity to WebhookEventDetails for display.
    /// </summary>
    private static WebhookEventDetails MapToDetails(WebhookEventEntity entity)
    {
        string payloadPreview = entity.Payload.Length > 200
            ? string.Concat(entity.Payload.AsSpan(0, 200), "...")
            : entity.Payload;

        return new WebhookEventDetails
        {
            Id = entity.Id,
            EventId = entity.EventId,
            EventType = entity.EventType,
            Source = entity.Source,
            TenantId = string.IsNullOrEmpty(entity.TenantId) ? null : entity.TenantId,
            Status = entity.Status.ToString(),
            RetryCount = entity.RetryCount,
            ErrorMessage = entity.ErrorMessage,
            ReceivedAt = entity.ReceivedAt,
            ProcessedAt = entity.ProcessedAt,
            PayloadPreview = payloadPreview
        };
    }
}
