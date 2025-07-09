namespace AppBlueprint.Contracts.Baseline.Notification.Requests;

public class UpdateNotificationRequest
{
    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
}
