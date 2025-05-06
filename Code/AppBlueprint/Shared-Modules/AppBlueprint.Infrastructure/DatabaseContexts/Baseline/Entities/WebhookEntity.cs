namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class WebhookEntity
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Secret { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
