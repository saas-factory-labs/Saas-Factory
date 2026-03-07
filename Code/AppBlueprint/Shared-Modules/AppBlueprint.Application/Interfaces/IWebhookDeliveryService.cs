namespace AppBlueprint.Application.Interfaces;

public interface IWebhookDeliveryService
{
    /// <summary>
    /// Delivers a webhook event to all registered endpoints for the given tenant that subscribe to the event type.
    /// </summary>
    /// <param name="eventType">The event type identifier (e.g. "subscription.created").</param>
    /// <param name="payload">The event payload to serialize as JSON.</param>
    /// <param name="tenantId">The tenant whose webhook endpoints should be notified.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeliverAsync(string eventType, object payload, string tenantId, CancellationToken cancellationToken = default);
}
