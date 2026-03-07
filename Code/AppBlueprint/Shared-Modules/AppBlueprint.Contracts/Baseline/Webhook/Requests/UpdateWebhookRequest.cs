namespace AppBlueprint.Contracts.Baseline.Webhook.Requests;

public sealed class UpdateWebhookRequest
{
    public required string Url { get; init; }
    public required string Secret { get; init; }
    public string? Description { get; init; }

    /// <summary>Comma-separated list of event types to subscribe to. Leave empty to subscribe to all events.</summary>
    public string EventTypes { get; init; } = string.Empty;
}
