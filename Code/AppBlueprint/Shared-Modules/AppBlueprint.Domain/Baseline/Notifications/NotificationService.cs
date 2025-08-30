namespace AppBlueprint.Domain.Baseline.Notifications;

public static class NotificationService
{
    public static Task SendEmailNotificationAsync(string recipientEmail, string subject, string body)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task SendSmsNotificationAsync(string phoneNumber, string message)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task SendPushNotificationAsync(string userId, string title, string message)
    {
        // Implementation pending
        throw new NotImplementedException();
    }

    public static Task<bool> IsNotificationPreferenceEnabledAsync(string userId, string notificationType)
    {
        // Implementation pending
        throw new NotImplementedException();
    }
}
