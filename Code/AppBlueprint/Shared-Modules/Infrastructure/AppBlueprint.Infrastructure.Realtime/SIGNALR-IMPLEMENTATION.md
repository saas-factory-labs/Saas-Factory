# SignalR Real-Time Communication Implementation

## Overview
AppBlueprint includes tenant-scoped SignalR infrastructure, a demo chat UI, and an optional conversation-authorization extension point for controlled cross-tenant messaging.

This file is the canonical reference for both the base SignalR implementation and the conversation authorization guidance that previously lived in `SIGNALR-AUTHORIZATION-GUIDE.md`.

## Current Implementation

### Shared modules

#### `TenantScopedHub<THub>`
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/SignalR/TenantScopedHub.cs`

Responsibilities:
- Extracts tenant and user context from claims.
- Falls back to `tenantId` and `userId` query-string values when claims are not available.
- Adds each connection to `tenant:{tenantId}` and `user:{userId}` groups.
- Exposes helper methods for tenant-scoped and user-scoped broadcasts.
- Aborts the connection if required tenant or user context cannot be resolved.

Key methods:

```csharp
protected string GetCurrentTenantId()
protected string GetCurrentUserId()
protected Task SendToTenantAsync(string method, object? arg1 = null, object? arg2 = null)
protected Task SendToUserAsync(string targetUserId, string method, object? arg1 = null, object? arg2 = null)
```

Claims currently checked:
- Tenant: `tenant_id`, `tid`, `TenantId`
- User: `sub`, `user_id`, `uid`

#### `DemoChatHub`
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/SignalR/DemoChatHub.cs`

Capabilities:
- Tenant-only broadcast chat.
- Direct messages to a specific user group.
- Cross-tenant conversation groups.
- Typing indicators.
- Online user tracking.
- Optional server-side authorization for conversation joins and sends.

Hub methods:

```csharp
Task SendMessageToTenant(string message)
Task SendDirectMessage(string recipientUserId, string message)
Task JoinConversation(string conversationId)
Task SendMessageToConversation(string conversationId, string message)
Task LeaveConversation(string conversationId)
Task NotifyTyping()
Task NotifyStoppedTyping()
```

Client events:

```text
ReceiveMessage
ReceiveDirectMessage
ReceiveConversationMessage
OnlineUsers
UserConnected
UserDisconnected
UserTyping
UserStoppedTyping
UserJoinedConversation
UserLeftConversation
```

Current implementation notes:
- Tenant chat is isolated through `tenant:{tenantId}` groups.
- Cross-tenant conversation access is checked only when an `IConversationAuthorizationService` is registered.
- Online users and conversation participants are tracked in memory for the demo hub.

#### `IConversationAuthorizationService`
Location: `Shared-Modules/AppBlueprint.Application/Interfaces/IConversationAuthorizationService.cs`

Purpose:
- Decide whether a user can join a conversation.
- Decide whether a user can send in a conversation.
- Return conversation IDs available to a user.

Interface:

```csharp
Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId)
Task<List<string>> GetUserConversationsAsync(string userId, string tenantId)
```

#### Example authorization services
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure/Services/ConversationAuthorizationService.cs`

Included sample implementations:
- `DatingAppConversationAuthorizationService`
- `PropertyRentalConversationAuthorizationService`

These are demo implementations only. They contain TODO comments where production code should query the database.

#### `AddAppBlueprintSignalR()`
Location: `Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/Extensions/RealtimeServiceCollectionExtensions.cs`

Registration:

```csharp
builder.Services.AddAppBlueprintSignalR();
```

Configured options:
- Client timeout: 60 seconds
- Handshake timeout: 15 seconds
- Keep-alive interval: 15 seconds
- Maximum receive message size: 1 MB
- MessagePack protocol enabled
- Detailed errors disabled by default

### Web host integration
Location: `AppBlueprint.Web/Program.cs`

Current registration and mapping:

```csharp
builder.Services.AddAppBlueprintSignalR();

app.MapHub<AppBlueprint.Infrastructure.Realtime.SignalR.DemoChatHub>("/hubs/demochat").AllowAnonymous();
app.MapHub<AppBlueprint.Infrastructure.Realtime.SignalR.NotificationHub>("/hubs/notifications").AllowAnonymous();
```

Important nuance:
- The hubs are currently mapped with `.AllowAnonymous()` to avoid OIDC redirect behavior during SignalR negotiation.
- The current demo relies on hub-side tenant and user resolution rather than middleware-level authorization.
- The `/realtime-chat` page itself is protected with `[Authorize]`.

### Demo UI
Location: `Shared-Modules/AppBlueprint.UiKit/Components/Pages/RealtimeChat.razor`

Route: `/realtime-chat`

Features:
- Authenticated demo page with connection status.
- Tenant-only mode and cross-tenant conversation mode.
- Automatic reconnect handling.
- Presence tracking and typing indicators.
- Conversation join and leave actions by conversation ID.
- Client-side connection setup using `/hubs/demochat?userId=...&tenantId=...`.

## Messaging Model

### Tenant-scoped flow
1. The user opens `/realtime-chat`.
2. The page loads the authenticated user context.
3. The client connects to `/hubs/demochat` and includes `userId` and `tenantId`.
4. `TenantScopedHub<THub>` resolves tenant and user context and adds the connection to tenant and user groups.
5. `SendMessageToTenant()` broadcasts only to `tenant:{tenantId}`.

### Conversation flow
1. The client calls `JoinConversation(conversationId)`.
2. `DemoChatHub` resolves the current tenant and user.
3. If `IConversationAuthorizationService` is registered, `CanJoinConversationAsync()` is called.
4. If authorized, the connection joins `conversation:{conversationId}`.
5. `SendMessageToConversation()` re-checks permission through `CanSendMessageAsync()` before broadcasting.

## Conversation Authorization

### When to use it
Use `IConversationAuthorizationService` when two parties should be able to communicate only under explicit business rules, for example:
- A dating app where only matched users can chat.
- A property app where property owners and interested renters can talk.
- A marketplace where buyers can only message sellers after a valid inquiry or purchase flow.

### Registration examples

Dating app:

```csharp
builder.Services.AddScoped<IConversationAuthorizationService, DatingAppConversationAuthorizationService>();
```

Property rental:

```csharp
builder.Services.AddScoped<IConversationAuthorizationService, PropertyRentalConversationAuthorizationService>();
```

Custom implementation:

```csharp
builder.Services.AddScoped<IConversationAuthorizationService, YourCustomAuthorizationService>();
```

### Example persistence patterns

Dating app match table:

```sql
CREATE TABLE matches (
    id UUID PRIMARY KEY,
    conversation_id VARCHAR(255) NOT NULL UNIQUE,
    user1_id VARCHAR(255) NOT NULL,
    user2_id VARCHAR(255) NOT NULL,
    tenant1_id VARCHAR(255) NOT NULL,
    tenant2_id VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    matched_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

Property inquiry table:

```sql
CREATE TABLE property_inquiries (
    id UUID PRIMARY KEY,
    property_id VARCHAR(255) NOT NULL,
    conversation_id VARCHAR(255) NOT NULL,
    property_owner_id VARCHAR(255) NOT NULL,
    interested_user_id VARCHAR(255) NOT NULL,
    interested_user_tenant_id VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);
```

Add indexes for `conversation_id`, owner and user lookup columns, and any active-status filters used by the authorization query.

### Implementation shape

```csharp
public sealed class YourAuthorizationService : IConversationAuthorizationService
{
    public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
    {
        // Query the database and return true only when the user is allowed.
    }

    public async Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId)
    {
        return await CanJoinConversationAsync(conversationId, userId, tenantId);
    }

    public async Task<List<string>> GetUserConversationsAsync(string userId, string tenantId)
    {
        // Return all conversation IDs available to the user.
    }
}
```

### Server-side rules
- Always authorize on the server, never only in the client.
- Prefer database-backed permissions over in-memory dictionaries.
- Log authorization failures.
- Add an audit trail if conversation access matters for compliance or support.

## Security and Production Caveats

The current implementation is useful, but it is not fully production hardened.

Current behavior to be aware of:
- `TenantScopedHub<THub>` accepts `tenantId` and `userId` from the query string when claims are unavailable.
- The Web host maps hubs with `.AllowAnonymous()`.
- `DemoChatHub` allows unrestricted conversation access when `IConversationAuthorizationService` is not registered.
- Conversation participant tracking is in memory only and does not survive restarts or scale-out.
- `SendDirectMessage()` assumes same-tenant targeting but does not verify the recipient against durable tenant data.

Recommended hardening before relying on this for production-sensitive messaging:
- Resolve tenant and user identity from authenticated claims or a signed or tokenized handshake instead of trusting raw query-string values.
- Replace demo authorization services with real database-backed implementations.
- Require authorization for conversation joins and sends in every deployment.
- Persist conversation membership or message history where needed.
- Add hub-level rate limiting and abuse monitoring.
- Add a Redis backplane if multiple web nodes will serve the same hubs.

## Usage Example

### Custom tenant-scoped hub

```csharp
using AppBlueprint.Infrastructure.Realtime.SignalR;

public sealed class NotificationHub : TenantScopedHub<NotificationHub>
{
    public async Task BroadcastNotification(string title, string message)
    {
        var payload = new
        {
            Title = title,
            Message = message,
            Timestamp = DateTime.UtcNow,
            UserId = GetCurrentUserId()
        };

        await SendToTenantAsync("ReceiveNotification", payload);
    }
}
```

### Client connection pattern used by the demo page

```csharp
var hubUrl = NavigationManager.ToAbsoluteUri(
    $"/hubs/demochat?userId={Uri.EscapeDataString(userId)}&tenantId={Uri.EscapeDataString(tenantId)}");

_hubConnection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();
```

## Testing Checklist

Build the affected projects:

```powershell
dotnet build Code/AppBlueprint/Shared-Modules/Infrastructure/AppBlueprint.Infrastructure.Realtime/AppBlueprint.Infrastructure.Realtime.csproj
dotnet build Code/AppBlueprint/Shared-Modules/AppBlueprint.UiKit/AppBlueprint.UiKit.csproj
dotnet build Code/AppBlueprint/AppBlueprint.Web/AppBlueprint.Web.csproj
```

Then verify:
1. Use the existing AppHost session if it is already running. Start it only if needed.
2. Open `/realtime-chat` as an authenticated user.
3. Confirm same-tenant users can see each other's tenant messages.
4. Confirm users in other tenants do not receive tenant-scoped messages.
5. Register an `IConversationAuthorizationService`, join a conversation such as `match-user1-user2` or `property-123`, and verify only authorized users can join and send.
6. Confirm unauthorized users receive a hub error when attempting to join or send.

## Framework vs Application Responsibility

Framework responsibilities:
- Base hub abstraction.
- Group management for tenant and user channels.
- Demo hubs and demo UI.
- SignalR registration and transport configuration.
- Conversation authorization extension point.

Application responsibilities:
- Real authorization rules for conversations.
- Persistence of messages, membership, and audit data.
- Rate limiting and operational monitoring.
- Domain-specific hubs and payloads.
- Any stronger identity validation beyond the demo connection pattern.

## Future Enhancements

Potential next steps:
- Redis backplane for multi-node scale-out.
- Message persistence abstraction.
- Hub rate limiting.
- Connection analytics and monitoring.
- Delivery receipts and offline queues.
- File and media support.
- WebRTC-based voice or video features.

## References

- [ASP.NET Core SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [SignalR authentication and authorization](https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz)
- [Managing SignalR groups](https://learn.microsoft.com/en-us/aspnet/core/signalr/groups)
- [Blazor with SignalR](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor)

---

Status: Implemented, with demo-oriented authorization and connection behavior that should be hardened before production use.
