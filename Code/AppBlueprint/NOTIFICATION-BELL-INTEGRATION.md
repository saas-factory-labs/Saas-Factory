# Notification Bell Integration Guide

This guide explains how to integrate the `NotificationBell` component into your application's main layout.

## Overview

The `NotificationBell` component provides:
- Real-time notification display via SignalR
- Unread notification count badge
- Dropdown list of recent notifications
- Mark as read functionality
- Click-to-navigate for actionable notifications
- Automatic reconnection handling

## Quick Start

### Option 1: Add to MainLayout

If your app uses a standard `MainLayout.razor`:

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<div class="page">
    <header class="app-header">
        <div class="header-left">
            <h1>Your App Name</h1>
        </div>
        <div class="header-right">
            @* Add NotificationBell here *@
            <NotificationBell />
            <UserMenu />
        </div>
    </header>
    
    <main>
        @Body
    </main>
</div>

@code {
    // Layout logic
}
```

### Option 2: Add to Custom Header Component

If you have a separate header/navbar component:

```razor
@* Components/Shared/AppHeader.razor *@
<header class="navbar">
    <div class="navbar-brand">
        <a href="/">Your App</a>
    </div>
    
    <div class="navbar-end">
        <NavLink href="/search">Search</NavLink>
        <NavLink href="/settings">Settings</NavLink>
        
        @* Add NotificationBell here *@
        <NotificationBell />
        
        <UserDropdown />
    </div>
</header>
```

Then use in layout:

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<div class="app-container">
    <AppHeader />
    <main>
        @Body
    </main>
</div>
```

## Component Features

### Real-Time Updates

The component automatically:
- Connects to SignalR hub on mount
- Joins user-specific notification group
- Receives real-time notifications
- Updates unread count instantly
- Handles reconnection on network issues

### User Interactions

Users can:
- Click bell icon to open dropdown
- View last 10 notifications
- Click "Mark all as read"
- Click individual notifications to mark as read and navigate
- See relative timestamps (e.g., "5m ago", "2h ago")

### Notification Types

Visual indicators for:
- ℹ️ Info (blue) - General information
- ✅ Success (green) - Successful actions
- ⚠️ Warning (yellow) - Warnings
- ❌ Error (red) - Error messages

## Styling

The component uses Tailwind CSS. Customize by:

### Override Colors

```css
/* In your global CSS */
.notification-bell {
    /* Change bell color */
    color: your-color;
}

.notification-bell-badge {
    /* Change badge color */
    background-color: your-color;
}
```

### Customize Dropdown

```css
.notification-dropdown {
    /* Change dropdown width */
    width: 400px;
    
    /* Change max height */
    max-height: 500px;
}
```

### Theme Integration

For dark mode support, add:

```razor
<NotificationBell class="@(isDarkMode ? "dark" : "")" />
```

And corresponding dark mode styles.

## Configuration

### Change Notification Limit

Edit `NotificationBell.razor`:

```csharp
private const int NotificationLimit = 20; // Default is 10
```

### Custom Empty State

Modify the empty state message:

```razor
<div class="notification-empty">
    <p>Your custom "No notifications" message</p>
</div>
```

### Add Action Buttons

Extend the dropdown footer:

```razor
<div class="notification-footer">
    <button @onclick="MarkAllAsRead">Mark all as read</button>
    <a href="/notifications">View all</a> @* Add this *@
    <a href="/notification-settings">Settings</a> @* Add this *@
</div>
```

## Advanced Usage

### Filtering Notifications

Add filter buttons to the dropdown:

```razor
@code {
    private NotificationType? filterType = null;
    
    private List<UserNotificationEntity> FilteredNotifications => 
        filterType.HasValue 
            ? notifications.Where(n => n.Type == filterType.Value).ToList()
            : notifications;
}

<div class="notification-filters">
    <button @onclick="() => filterType = null">All</button>
    <button @onclick="() => filterType = NotificationType.Info">Info</button>
    <button @onclick="() => filterType = NotificationType.Success">Success</button>
</div>
```

### Sound Notifications

Play sound on new notification:

```csharp
private async Task OnNotificationReceived(object sender, UserNotificationEntity notification)
{
    await InvokeAsync(async () =>
    {
        notifications.Insert(0, notification);
        unreadCount++;
        
        // Play notification sound
        await JS.InvokeVoidAsync("playNotificationSound");
        
        StateHasChanged();
    });
}
```

Add JavaScript function:

```javascript
// wwwroot/js/notifications.js
window.playNotificationSound = function() {
    const audio = new Audio('/sounds/notification.mp3');
    audio.volume = 0.5;
    audio.play().catch(err => console.log('Audio play failed:', err));
};
```

### Desktop Notifications

Show browser notification when tab is not active:

```csharp
private async Task OnNotificationReceived(object sender, UserNotificationEntity notification)
{
    await InvokeAsync(async () =>
    {
        notifications.Insert(0, notification);
        unreadCount++;
        
        // Show desktop notification if tab is not active
        await JS.InvokeVoidAsync("showDesktopNotification", 
            notification.Title, 
            notification.Message);
        
        StateHasChanged();
    });
}
```

JavaScript helper:

```javascript
window.showDesktopNotification = function(title, body) {
    if (!document.hasFocus() && Notification.permission === 'granted') {
        new Notification(title, {
            body: body,
            icon: '/icon-192.png',
            badge: '/badge-72.png'
        });
    }
};
```

## Testing

### Test with Demo Page

1. Navigate to `/demo/notifications`
2. Register for push notifications
3. Send test notification to your user
4. Verify bell icon updates with badge
5. Click bell to see notification in dropdown

### Manual SignalR Test

Use browser console:

```javascript
// Test SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notifications')
    .build();

await connection.start();
console.log('Connected to NotificationHub');

// Send test notification (from server-side code)
```

## Troubleshooting

### Bell Icon Not Showing

- Verify `NotificationBell` component is in layout
- Check browser console for import errors
- Ensure Tailwind CSS is loaded

### No Real-Time Updates

- Check SignalR hub is mapped in `Program.cs`
- Verify authentication is working (check `HttpContext.User`)
- Check browser console for SignalR connection errors
- Ensure `AddNotificationServices()` is called

### Badge Not Updating

- Verify `INotificationService` is injected correctly
- Check `GetUnreadCountAsync` returns correct count
- Ensure database has notification records
- Check TenantId is set correctly

### Notifications Not Marking as Read

- Verify `MarkAsReadAsync` calls are completing
- Check database updates are persisting
- Ensure userId matches authenticated user
- Check error logs for exceptions

## Accessibility

The component includes:
- ARIA labels for screen readers
- Keyboard navigation support (Tab, Enter, Escape)
- Focus management for dropdown
- Semantic HTML structure

Enhance accessibility:

```razor
<button 
    class="notification-bell-btn"
    aria-label="@($"Notifications, {unreadCount} unread")"
    aria-expanded="@isOpen"
    aria-haspopup="true">
    @* Bell icon *@
</button>
```

## Performance Considerations

- Component loads last 10 notifications on mount
- SignalR connection shared across instances
- Unread count cached in state
- Dropdown rendered on-demand (not hidden with CSS)

For better performance with many notifications:
- Implement virtual scrolling
- Lazy load notification details
- Debounce mark-as-read calls
- Use pagination instead of loading all

## Related Documentation

- [FIREBASE-SETUP-GUIDE.md](./FIREBASE-SETUP-GUIDE.md) - Firebase push notification setup
- [NotificationDemo.razor](./AppBlueprint.Web/Components/Pages/Demo/NotificationDemo.razor) - Full demo example
- [NotificationHub.cs](./Shared-Modules/AppBlueprint.Infrastructure/Services/Notifications/NotificationHub.cs) - SignalR hub implementation

## Next Steps

1. Add NotificationBell to your layout (see examples above)
2. Test real-time notifications using demo page
3. Customize styling to match your app's theme
4. Set up Firebase for push notifications (see FIREBASE-SETUP-GUIDE.md)
5. Create notification-sending triggers in your business logic
