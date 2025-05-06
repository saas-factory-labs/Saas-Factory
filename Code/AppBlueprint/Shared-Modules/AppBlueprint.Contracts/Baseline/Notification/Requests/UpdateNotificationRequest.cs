namespace AppBlueprint.Contracts.Baseline.Notification.Requests;

public class UpdateNotificationRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
