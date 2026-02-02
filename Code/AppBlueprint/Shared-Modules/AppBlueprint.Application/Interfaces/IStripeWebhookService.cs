namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for processing Stripe webhook events.
/// Handles signature verification, idempotency, event storage, and business logic dispatch.
/// </summary>
public interface IStripeWebhookService
{
    /// <summary>
    /// Processes a Stripe webhook event.
    /// Verifies signature, checks for duplicates, stores event, and dispatches to handlers.
    /// </summary>
    /// <param name="payload">Raw webhook payload (JSON string).</param>
    /// <param name="signatureHeader">Stripe-Signature header value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if event was processed successfully, false otherwise.</returns>
    Task<WebhookProcessingResult> ProcessWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries processing of failed webhook events.
    /// </summary>
    /// <param name="maxRetries">Maximum number of retry attempts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of events successfully retried.</returns>
    Task<int> RetryFailedEventsAsync(int maxRetries = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets webhook event by ID.
    /// </summary>
    Task<WebhookEventDetails?> GetWebhookEventByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets webhook events for a specific tenant.
    /// </summary>
    Task<IEnumerable<WebhookEventDetails>> GetWebhookEventsByTenantIdAsync(string tenantId, int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent webhook events.
    /// </summary>
    Task<IEnumerable<WebhookEventDetails>> GetRecentWebhookEventsAsync(int maxResults = 50, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of webhook processing.
/// </summary>
public sealed class WebhookProcessingResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public bool WasDuplicate { get; init; }
    public string? EventId { get; init; }
    public string? EventType { get; init; }

    public static WebhookProcessingResult Successful(string eventId, string eventType, bool wasDuplicate = false)
    {
        return new WebhookProcessingResult
        {
            Success = true,
            EventId = eventId,
            EventType = eventType,
            WasDuplicate = wasDuplicate
        };
    }

    public static WebhookProcessingResult Failed(string errorMessage)
    {
        return new WebhookProcessingResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// Details of a webhook event for display/querying.
/// </summary>
public sealed class WebhookEventDetails
{
    public required string Id { get; init; }
    public required string EventId { get; init; }
    public required string EventType { get; init; }
    public required string Source { get; init; }
    public string? TenantId { get; init; }
    public required string Status { get; init; }
    public int RetryCount { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime ReceivedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? PayloadPreview { get; init; }
}
