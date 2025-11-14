namespace AppBlueprint.Contracts.Baseline.Notification.Responses;

public class NotificationResponse
{
    public string Id { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
