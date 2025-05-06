namespace AppBlueprint.Contracts.Baseline.Notification.Requests;

public class CreateNotificationRequest
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
