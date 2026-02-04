# Firebase Notifications - Quick Reference

## Quick Start (5 Minutes)

### 1. Test with Demo Page
```
Navigate to: /demo/notifications
```
1. Click "Register for Push Notifications" → Grant permissions
2. Send test notification (select channels: InApp + Push)
3. Verify notification appears in bell icon
4. Check browser shows push notification

### 2. Add Bell to Layout
```razor
@* MainLayout.razor *@
<header>
    <NotificationBell />  @* Add this line *@
</header>
```

### 3. Send Notification from Code
```csharp
// Inject INotificationService
await _notificationService.SendAsync(new SendNotificationRequest(
    userId: "usr_...",
    title: "New Message",
    message: "You have a new message from John",
    type: NotificationType.Info,
    actionUrl: "/messages",
    channels: NotificationChannels.InApp | NotificationChannels.Push
));
```

Done! ✅

---

## Common Tasks

### Send In-App Notification Only
```csharp
await _notificationService.SendAsync(new SendNotificationRequest(
    userId: userId,
    title: "Profile Updated",
    message: "Your profile has been updated successfully",
    type: NotificationType.Success,
    actionUrl: "/profile",
    channels: NotificationChannels.InApp  // Only SignalR
));
```

### Send Push Notification with Custom Data
```csharp
await _pushNotificationService.SendAsync(new PushNotificationRequest(
    userId: userId,
    title: "Payment Received",
    body: "$50.00 received from customer",
    imageUrl: "https://example.com/payment-icon.png",
    actionUrl: "/payments/12345",
    data: new Dictionary<string, string>
    {
        ["type"] = "payment",
        ["amount"] = "50.00",
        ["currency"] = "USD",
        ["payment_id"] = "12345"
    }
));
```

### Register FCM Token (from Blazor)
```csharp
@inject IJSRuntime JS
@inject IPushNotificationService PushService

private async Task RegisterForPushNotificationsAsync()
{
    // Get FCM token from Firebase SDK
    string token = await JS.InvokeAsync<string>("requestFirebaseTokenAndPermission", vapidKey);
    
    if (!string.IsNullOrEmpty(token))
    {
        // Register with backend
        await PushService.RegisterTokenAsync(new RegisterPushTokenRequest(
            userId: currentUserId,
            token: token,
            platform: DeviceType.Web
        ));
    }
}
```

### Update User Preferences
```csharp
// Get or create preferences
var prefs = await _preferencesRepo.GetByUserIdAsync(userId);
if (prefs is null)
{
    prefs = NotificationPreferencesEntity.CreateDefault(userId, tenantId);
}

// Disable push notifications
prefs.EnablePush = false;

// Set quiet hours (10 PM to 8 AM)
prefs.SetQuietHours(new TimeSpan(22, 0, 0), new TimeSpan(8, 0, 0));

await _preferencesRepo.UpdateAsync(prefs);
```

### Get Unread Count
```csharp
int unreadCount = await _notificationService.GetUnreadCountAsync(userId);
```

### Mark All as Read
```csharp
await _notificationService.MarkAllAsReadAsync(userId);
```

### Get Recent Notifications
```csharp
List<UserNotificationEntity> notifications = await _notificationService.GetUserNotificationsAsync(userId, limit: 20);
```

---

## Notification Types

```csharp
NotificationType.Info     // ℹ️ Blue - General information
NotificationType.Success  // ✅ Green - Successful actions
NotificationType.Warning  // ⚠️ Yellow - Warnings, important notices
NotificationType.Error    // ❌ Red - Errors, failures
```

---

## Notification Channels

```csharp
NotificationChannels.InApp  // SignalR real-time
NotificationChannels.Push   // Firebase Cloud Messaging
NotificationChannels.Email  // Email (interface only - not implemented)
NotificationChannels.SMS    // SMS (interface only - not implemented)

// Multiple channels (flags enum)
NotificationChannels.InApp | NotificationChannels.Push
```

---

## Service Interfaces

### INotificationService (Main Orchestrator)
```csharp
Task SendAsync(SendNotificationRequest request);
Task<List<UserNotificationEntity>> GetUserNotificationsAsync(string userId, int limit = 10);
Task<int> GetUnreadCountAsync(string userId);
Task MarkAsReadAsync(string notificationId);
Task MarkAllAsReadAsync(string userId);
```

### IPushNotificationService (FCM)
```csharp
Task SendAsync(PushNotificationRequest request);
Task RegisterTokenAsync(RegisterPushTokenRequest request);
Task UnregisterTokenAsync(string userId, string token);
```

### IInAppNotificationService (SignalR)
```csharp
Task SendAsync(SendNotificationRequest request);
```

---

## JavaScript Interop (Blazor)

### Initialize Firebase
```javascript
await JS.InvokeVoidAsync("initializeFirebaseMessaging", firebaseConfig, vapidKey);
```

### Get FCM Token
```javascript
string token = await JS.InvokeAsync<string>("getFirebaseToken");
```

### Request Permissions + Token
```javascript
string token = await JS.InvokeAsync<string>("requestFirebaseTokenAndPermission", vapidKey);
```

### Setup Foreground Handler
```javascript
await JS.InvokeVoidAsync("setupFirebaseForegroundHandler");
```

---

## SignalR Events (Client-Side)

### Subscribe to Notifications
```csharp
hubConnection.On<UserNotificationEntity>("ReceiveNotification", notification =>
{
    // Handle new notification
    notifications.Insert(0, notification);
    unreadCount++;
    StateHasChanged();
});
```

### Connection States
```csharp
hubConnection.Closed += async (error) =>
{
    await Task.Delay(new Random().Next(0, 5) * 1000);
    await hubConnection.StartAsync();
};
```

---

## Configuration

### Environment Variables (Required)
```bash
FIREBASE_CREDENTIALS='{"type":"service_account","project_id":"...","private_key":"..."}'
FIREBASE_VAPID_KEY="YOUR_VAPID_KEY"
```

### Firebase Config (firebase-messaging-sw.js)
```javascript
const firebaseConfig = {
  apiKey: "YOUR_API_KEY",
  authDomain: "YOUR_PROJECT_ID.firebaseapp.com",
  projectId: "YOUR_PROJECT_ID",
  storageBucket: "YOUR_PROJECT_ID.appspot.com",
  messagingSenderId: "YOUR_MESSAGING_SENDER_ID",
  appId: "YOUR_APP_ID"
};
```

---

## Database Queries

### Get Notifications for User
```csharp
var notifications = await _context.UserNotifications
    .Where(n => n.UserId == userId && n.TenantId == tenantId)
    .OrderByDescending(n => n.CreatedAt)
    .Take(10)
    .ToListAsync();
```

### Get Unread Count
```csharp
var count = await _context.UserNotifications
    .Where(n => n.UserId == userId && n.TenantId == tenantId && !n.IsRead)
    .CountAsync();
```

### Mark as Read with ExecuteUpdate
```csharp
await _context.UserNotifications
    .Where(n => n.UserId == userId && n.TenantId == tenantId)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(n => n.IsRead, true)
        .SetProperty(n => n.ReadAt, DateTimeOffset.UtcNow));
```

---

## Error Handling

### FCM Token Errors (Auto-Handled)
```csharp
// These errors automatically deactivate tokens:
- InvalidArgument
- Unregistered
- SenderIdMismatch
```

### SignalR Reconnection (Auto-Handled)
```csharp
// NotificationBell.razor automatically reconnects on:
- Network issues
- Server restart
- Connection timeout
```

---

## Testing Checklist

- [ ] Navigate to `/demo/notifications`
- [ ] Click "Register for Push Notifications"
- [ ] Grant browser notification permissions
- [ ] Send test notification (InApp + Push)
- [ ] Verify bell icon shows badge count
- [ ] Click bell icon to see dropdown
- [ ] Click notification to navigate
- [ ] Click "Mark all as read"
- [ ] Send another test notification
- [ ] Verify real-time update in bell
- [ ] Close browser tab
- [ ] Send notification (should see browser push)
- [ ] Click push notification
- [ ] Verify navigation works

---

## Troubleshooting

### No Notifications Appearing
1. Check SignalR connection: DevTools → Network → WS tab
2. Verify user is authenticated: Check `HttpContext.User.Identity.IsAuthenticated`
3. Check database: `SELECT * FROM user_notifications WHERE user_id = 'usr_...'`
4. Check logs for errors

### Push Not Working
1. Verify `FIREBASE_CREDENTIALS` is set correctly
2. Check VAPID key is valid
3. Verify browser permissions granted
4. Check FCM token registered: `SELECT * FROM push_notification_tokens WHERE user_id = 'usr_...'`
5. Check firebase-messaging-sw.js is registered
6. Ensure HTTPS (required for service workers)

### Bell Icon Not Updating
1. Check SignalR connection state
2. Verify `INotificationService` injected
3. Check `OnInitializedAsync` is called
4. Check console for JavaScript errors
5. Verify TenantId matches

---

## Performance Tips

- Load last N notifications (default 10, not all)
- Use ExecuteUpdateAsync for bulk mark-as-read
- Batch FCM sends (MulticastMessage supports 500 tokens)
- Index frequently queried columns (UserId, TenantId, IsRead)
- Consider pagination for notification history

---

## Security Notes

- All queries filtered by TenantId (multi-tenancy)
- Users can only access their own notifications
- SignalR groups are user-specific (user_{userId})
- FCM tokens validated and cleaned up automatically
- HTTPS required in production (service workers)

---

## Documentation Links

- **Setup Guide**: [FIREBASE-SETUP-GUIDE.md](./FIREBASE-SETUP-GUIDE.md)
- **Integration Guide**: [NOTIFICATION-BELL-INTEGRATION.md](./NOTIFICATION-BELL-INTEGRATION.md)
- **Full Summary**: [NOTIFICATION-SYSTEM-SUMMARY.md](./NOTIFICATION-SYSTEM-SUMMARY.md)
- **Demo Page**: Navigate to `/demo/notifications`

---

## Support

Questions? Check:
1. Demo page at `/demo/notifications`
2. FIREBASE-SETUP-GUIDE.md for configuration
3. NOTIFICATION-BELL-INTEGRATION.md for UI usage
4. Application logs for errors
5. Browser console for client-side issues
