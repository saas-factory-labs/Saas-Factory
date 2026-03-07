namespace AppBlueprint.Contracts.Baseline.Webhook.Responses;

public sealed class WebhookResponse
{
    public required string Id { get; init; }
    public required string Url { get; init; }
    public string? Description { get; init; }

    /// <summary>Comma-separated list of subscribed event types. Empty means all events.</summary>
    public string EventTypes { get; init; } = string.Empty;

    public required string TenantId { get; init; }
}
