using AppBlueprint.Domain.Entities.Webhooks;

namespace AppBlueprint.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for webhook event entity operations.
/// </summary>
public interface IWebhookEventRepository
{
    /// <summary>
    /// Gets a webhook event by its external event ID and source.
    /// Used for idempotency checking.
    /// </summary>
    Task<WebhookEventEntity?> GetByEventIdAsync(string eventId, string source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a webhook event by its internal ID.
    /// </summary>
    Task<WebhookEventEntity?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all webhook events for a specific tenant.
    /// </summary>
    Task<IEnumerable<WebhookEventEntity>> GetByTenantIdAsync(string tenantId, int pageSize = 50, int pageNumber = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets webhook events by status.
    /// </summary>
    Task<IEnumerable<WebhookEventEntity>> GetByStatusAsync(WebhookEventStatus status, int maxResults = 100, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed webhook events that are eligible for retry.
    /// </summary>
    Task<IEnumerable<WebhookEventEntity>> GetFailedEventsForRetryAsync(int maxRetries = 3, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new webhook event.
    /// </summary>
    Task<WebhookEventEntity> AddAsync(WebhookEventEntity webhookEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing webhook event.
    /// </summary>
    Task UpdateAsync(WebhookEventEntity webhookEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes old webhook events (for cleanup).
    /// </summary>
    Task DeleteOldEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}
