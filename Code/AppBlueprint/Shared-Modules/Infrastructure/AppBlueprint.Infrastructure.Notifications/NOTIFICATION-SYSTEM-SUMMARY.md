# Notification System Guide

## Overview
AppBlueprint currently ships a notification stack built around:

- persisted user notifications in the database
- SignalR delivery for in-app real-time updates
- optional Firebase Cloud Messaging push delivery
- a reusable bell component and an authenticated demo page in `AppBlueprint.Web`

This file is the canonical reference for the notification system summary, bell integration notes, quick reference, and the older multi-channel comparison content that previously lived in separate files.

## Current Implementation

### What is implemented

- `INotificationService` is the main application-facing API for user notifications.
- In-app notifications are persisted as `UserNotificationEntity` records.
- New in-app notifications are also pushed over SignalR through `NotificationHub`.
- Push notifications can be sent through Firebase Cloud Messaging when Firebase Admin configuration is available.
- User notification preferences and quiet hours are stored in `NotificationPreferencesEntity`.
- Device tokens are stored in `PushNotificationTokenEntity`.
- A reusable `NotificationBell.razor` component exists for UI integration.
- An authenticated `/demo/notifications` page exists for manual testing.

### What is partial or not implemented

- `NotificationChannels.Email` and `NotificationChannels.Sms` are present in the API but are not implemented yet.
- `IMultiChannelNotificationService` exists, but it is not registered by default in the base notification registration.
- Tenant-wide in-app broadcasts from `IMultiChannelNotificationService.SendTenantNotificationAsync(...)` are real-time broadcasts only and do not persist per-user notification rows.
- The hub exposes `MarkNotificationAsRead(...)`, but read-state changes are currently handled through `INotificationService`, not through the hub method.

## Key Components

### Application contracts

- `Shared-Modules/AppBlueprint.Application/Interfaces/INotificationService.cs`
  - Send a user notification
  - Query recent notifications
  - Query unread count
  - Mark one or all notifications as read
- `Shared-Modules/AppBlueprint.Application/Interfaces/IInAppNotificationService.cs`
  - Persist and fan out in-app notifications
- `Shared-Modules/AppBlueprint.Application/Interfaces/IPushNotificationService.cs`
  - Send FCM push notifications
  - Register and unregister device tokens
- `Shared-Modules/AppBlueprint.Application/Interfaces/IMultiChannelNotificationService.cs`
  - Optional orchestration layer for per-user and per-tenant multi-channel sends

### Domain entities

- `Shared-Modules/AppBlueprint.Domain/Entities/Notifications/UserNotificationEntity.cs`
  - Notification history per user
- `Shared-Modules/AppBlueprint.Domain/Entities/Notifications/NotificationPreferencesEntity.cs`
  - Channel preferences and quiet hours
- `Shared-Modules/AppBlueprint.Domain/Entities/Notifications/PushNotificationTokenEntity.cs`
  - Active device tokens per user and tenant

### Persistence repositories

- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/Repositories/UserNotificationRepository.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/Repositories/UserNotificationPreferencesRepository.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Persistence/Repositories/UserPushNotificationTokenRepository.cs`

### Infrastructure services

- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/NotificationService.cs`
  - Main `INotificationService` implementation
  - Reads preferences
  - Creates default preferences on first use
  - Applies quiet-hours suppression
  - Sends through in-app and push channels when enabled
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/Notifications/InAppNotificationService.cs`
  - Persists the notification record
  - Emits the `ReceiveNotification` SignalR event to the target user group
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/FirebasePushNotificationService.cs`
  - Sends FCM multicast notifications to active tokens
  - Deactivates invalid tokens on failed sends
  - Supports direct user sends and tenant-wide sends
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/Notifications/MultiChannelNotificationService.cs`
  - Optional orchestration service for explicit multi-channel sends
  - Logs TODOs for email and SMS

### Web integration

- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/SignalR/NotificationHub.cs`
  - Tenant-aware SignalR hub for `ReceiveNotification`
- `AppBlueprint.Web/Program.cs`
  - Maps the hub at `/hubs/notifications`
- `AppBlueprint.Web/Components/Shared/NotificationBell.razor`
  - Reusable bell/dropdown component
- `AppBlueprint.Web/Components/Pages/Demo/NotificationDemo.razor`
  - Manual test page at `/demo/notifications`
- `AppBlueprint.Web/Controllers/FirebaseConfigController.cs`
  - Exposes public Firebase browser config and VAPID key
- `AppBlueprint.Web/TypeScript/firebase-messaging-helper.ts`
  - Browser helper for permission and token registration
- `AppBlueprint.Web/wwwroot/firebase-messaging-sw.js`
  - Service worker for background push handling

## Registration and Runtime Flow

### Default DI registration

`AppBlueprint.Infrastructure.Extensions.ServiceCollectionExtensions.AddAppBlueprintInfrastructure(...)` calls `AddNotificationServices()`.

`AddNotificationServices()` currently registers:

- `INotificationRepository`
- `INotificationPreferencesRepository`
- `IPushNotificationTokenRepository`
- `INotificationService`
- `IPushNotificationService`
- `IInAppNotificationService`
- SignalR services

Important:
- `IMultiChannelNotificationService` is not part of this default registration.
- If you want to inject `IMultiChannelNotificationService`, also call `services.AddMultiChannelNotifications();`

### Main send flow

For the common user-notification path:

1. Call `INotificationService.SendAsync(...)`.
2. `NotificationService` loads or creates preferences for the target user.
3. If the user is in quiet hours, the send returns without delivering.
4. If `NotificationChannels.InApp` is requested and enabled, the notification is persisted and delivered over SignalR.
5. If `NotificationChannels.Push` is requested and enabled, the notification is sent to active FCM tokens.

Practical guidance:
- Prefer explicit channel flags instead of relying on `NotificationChannels.All`.
- Today, only `InApp` and `Push` are live delivery channels.

## Quick Reference

### Send an in-app notification

```csharp
await notificationService.SendAsync(new SendNotificationRequest(
    tenantId,
    userId,
    "Export ready",
    "Your export finished successfully.",
    NotificationType.Success,
    new Uri("/exports", UriKind.Relative),
    NotificationChannels.InApp));
```

### Send in-app plus push

```csharp
await notificationService.SendAsync(new SendNotificationRequest(
    tenantId,
    userId,
    "Build completed",
    "The deployment pipeline finished.",
    NotificationType.Info,
    null,
    NotificationChannels.InApp | NotificationChannels.Push));
```

### Query recent notifications and unread count

```csharp
IEnumerable<UserNotificationEntity> notifications =
    await notificationService.GetUserNotificationsAsync(userId, 20);

int unreadCount = await notificationService.GetUnreadCountAsync(userId);
```

### Mark notifications as read

```csharp
await notificationService.MarkAsReadAsync(notificationId);
await notificationService.MarkAllAsReadAsync(userId);
```

### Register a push token

```csharp
await pushNotificationService.RegisterTokenAsync(new RegisterPushTokenRequest(
    tenantId,
    userId,
    token,
    DeviceType.Web));
```

### Optional multi-channel orchestration

First register the service:

```csharp
services.AddMultiChannelNotifications();
```

Then use it:

```csharp
await multiChannelNotificationService.SendNotificationAsync(
    tenantId,
    userId,
    "New message",
    "You have received a new message.",
    NotificationType.Info,
    NotificationChannels.InApp | NotificationChannels.Push);
```

## UI Integration

### Bell component

The reusable bell component lives at:

- `AppBlueprint.Web/Components/Shared/NotificationBell.razor`

What it does:

- loads the latest notifications through `INotificationService`
- shows unread count
- marks notifications as read on click
- opens a SignalR connection to `/hubs/notifications`
- links to `/demo/notifications` from its footer

No current bell call sites were found in `AppBlueprint.Web`, so integrating it into a layout or header still requires an explicit component reference.

Minimal usage:

```razor
@using AppBlueprint.Web.Components.Shared

<NotificationBell />
```

Operational expectations:

- the current user must be authenticated
- the user identity must resolve to a usable `sub` or `Identity.Name`
- the notification services must already be registered through the infrastructure setup

### Demo page

The demo page lives at:

- `AppBlueprint.Web/Components/Pages/Demo/NotificationDemo.razor`

Route:

- `/demo/notifications`

It is the best current reference for end-to-end notification behavior in the web app because it already exercises:

- authenticated user and tenant resolution
- SignalR connection status
- manual notification sends
- browser permission requests
- Firebase token generation
- token registration through `IPushNotificationService`

## Runtime Endpoints

- Notification hub: `/hubs/notifications`
- Demo page: `/demo/notifications`
- Firebase browser config: `/api/firebase-config`
- Firebase VAPID key: `/api/firebase-config/vapid-key`

For tenant-scoped SignalR connection behavior and current authorization posture, see [SIGNALR-IMPLEMENTATION.md](../AppBlueprint.Infrastructure.Realtime/SIGNALR-IMPLEMENTATION.md).

## Firebase Configuration

There are currently two separate Firebase configuration surfaces in the notification stack.

### Browser-side web push configuration

`FirebaseConfigController` serves public browser configuration from these keys:

- `FIREBASE_API_KEY`
- `FIREBASE_AUTH_DOMAIN`
- `FIREBASE_PROJECT_ID`
- `FIREBASE_STORAGE_BUCKET`
- `FIREBASE_MESSAGING_SENDER_ID`
- `FIREBASE_APP_ID`
- `FIREBASE_VAPID_KEY`

These values are intentionally public browser configuration, not secrets.

### Server-side Firebase Admin configuration

`FirebasePushNotificationService` currently initializes Firebase Admin from:

- `Firebase:CredentialsJson`
- `Firebase:ProjectId`

Important distinction:

- The browser path uses flat `FIREBASE_*` keys.
- The server push sender currently reads the `Firebase` configuration section.
- Older notification docs that only mention `FIREBASE_CREDENTIALS` are stale relative to the current implementation.

Practical implication:

- If you want browser push permission and token generation to work, the web app must see the `FIREBASE_*` keys above.
- If you want server-side push sends to work, the process also needs configuration bound into `Firebase:CredentialsJson` and `Firebase:ProjectId`.

### Service worker behavior

The current service worker does not hardcode Firebase client config.

Instead:

1. `firebase-messaging-helper.ts` fetches config from `/api/firebase-config`
2. the browser registers `/firebase-messaging-sw.js`
3. the service worker fetches the same server-provided config during activation

Do not follow older guidance that assumes Firebase config is embedded directly in `firebase-messaging-sw.js`.

### AppHost note

`AppBlueprint.AppHost/Program.cs` explicitly forwards Logto and database settings, but it does not currently add the Firebase notification keys with `WithEnvironment(...)`.

That means you should verify that Firebase notification settings are available to the web process through your actual runtime setup.

## Current Limitations and Design Status

The older comparison document has been superseded by the current implementation direction:

- SignalR is the in-app real-time transport.
- Persisted database notifications are the source of truth for per-user history.
- Firebase Cloud Messaging is the current push channel.
- Email and SMS remain extension points, not finished channels.

Known implementation caveats:

- `NotificationHub` is currently mapped with `AllowAnonymous()` in `AppBlueprint.Web/Program.cs`, and the hub class has its `[Authorize]` attribute commented out.
- `IMultiChannelNotificationService.SendTenantNotificationAsync(...)` broadcasts in-app notifications to the tenant SignalR group but does not create per-user notification history rows.
- The reusable bell component exists, but it is not currently wired into the web shell by default.

## Related Files

- `AppBlueprint.Web/Components/Shared/NotificationBell.razor`
- `AppBlueprint.Web/Components/Pages/Demo/NotificationDemo.razor`
- `AppBlueprint.Web/Controllers/FirebaseConfigController.cs`
- `Shared-Modules/AppBlueprint.Application/Interfaces/INotificationService.cs`
- `Shared-Modules/AppBlueprint.Application/Interfaces/IPushNotificationService.cs`
- `Shared-Modules/AppBlueprint.Application/Interfaces/IMultiChannelNotificationService.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/NotificationService.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Notifications/FirebasePushNotificationService.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/Notifications/InAppNotificationService.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/Notifications/MultiChannelNotificationService.cs`
- `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/SignalR/NotificationHub.cs`

## Summary

Use this guide as the single source of truth for the current notification system.

The practical current model is:

- `INotificationService` for normal app-facing user notifications
- SignalR for in-app real-time delivery
- persisted notification rows for history and unread counts
- Firebase Cloud Messaging for optional push delivery
- `NotificationBell.razor` and `/demo/notifications` as the current UI touchpoints

The older specialized notification docs are retained only as compatibility pointers.
