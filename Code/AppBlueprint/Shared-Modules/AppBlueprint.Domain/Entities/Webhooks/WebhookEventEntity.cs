using AppBlueprint.SharedKernel;

namespace AppBlueprint.Domain.Entities.Webhooks;

/// <summary>
/// Domain entity representing a webhook event received from external services.
/// Stores webhook events for idempotency checking, auditing, and retry logic.
/// </summary>
public sealed class WebhookEventEntity : BaseEntity, ITenantScoped
{
    public WebhookEventEntity()
    {
        Id = PrefixedUlid.Generate("whevt");
        EventId = string.Empty;
        EventType = string.Empty;
        Source = string.Empty;
        Payload = string.Empty;
        TenantId = string.Empty;
        Status = WebhookEventStatus.Pending;
        RetryCount = 0;
    }

    public WebhookEventEntity(string eventId, string eventType, string source, string payload, string? tenantId = null) : this()
    {
        EventId = eventId ?? throw new ArgumentNullException(nameof(eventId));
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        TenantId = tenantId ?? string.Empty; // Some webhooks may not be tenant-scoped
    }

    /// <summary>
    /// External event ID from the webhook provider (e.g., Stripe event ID).
    /// Used for idempotency checking.
    /// </summary>
    public string EventId { get; private set; }

    /// <summary>
    /// Type of the webhook event (e.g., "payment_intent.succeeded").
    /// </summary>
    public string EventType { get; private set; }

    /// <summary>
    /// Source of the webhook (e.g., "stripe", "paypal").
    /// </summary>
    public string Source { get; private set; }

    /// <summary>
    /// Raw JSON payload of the webhook event.
    /// </summary>
    public string Payload { get; private set; }

    /// <summary>
    /// Tenant ID if the webhook is tenant-scoped.
    /// Empty string for global webhooks.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Processing status of the webhook event.
    /// </summary>
    public WebhookEventStatus Status { get; private set; }

    /// <summary>
    /// Number of times processing has been attempted.
    /// </summary>
    public int RetryCount { get; private set; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Timestamp when the event was received.
    /// </summary>
    public DateTime ReceivedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the event was processed successfully.
    /// </summary>
    public DateTime? ProcessedAt { get; private set; }

    /// <summary>
    /// Marks the event as successfully processed.
    /// </summary>
    public void MarkAsProcessed()
    {
        Status = WebhookEventStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    /// <summary>
    /// Marks the event as failed with an error message.
    /// </summary>
    public void MarkAsFailed(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);

        Status = WebhookEventStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    /// <summary>
    /// Marks the event as pending retry.
    /// </summary>
    public void MarkAsPendingRetry()
    {
        Status = WebhookEventStatus.PendingRetry;
        RetryCount++;
    }

    /// <summary>
    /// Marks the event as permanently failed (max retries exceeded).
    /// </summary>
    public void MarkAsPermanentlyFailed(string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(errorMessage);

        Status = WebhookEventStatus.PermanentlyFailed;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Status of a webhook event.
/// </summary>
public enum WebhookEventStatus
{
    /// <summary>
    /// Event received but not yet processed.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Event successfully processed.
    /// </summary>
    Processed = 1,

    /// <summary>
    /// Event processing failed but will be retried.
    /// </summary>
    PendingRetry = 2,

    /// <summary>
    /// Event processing failed.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Event processing failed permanently (max retries exceeded).
    /// </summary>
    PermanentlyFailed = 4
}
