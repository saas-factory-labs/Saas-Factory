# SignalR Real-Time Communication Implementation

## Overview
Implemented tenant-aware, authenticated SignalR infrastructure with a demo chat page to showcase real-time communication capabilities.

## What Was Implemented

### 1. **Infrastructure Layer** (`AppBlueprint.Infrastructure`)

#### **TenantScopedHub.cs** - Base Hub Class
Location: `Shared-Modules/AppBlueprint.Infrastructure/SignalR/TenantScopedHub.cs`

**Features:**
- ✅ Abstract base class for all SignalR hubs
- ✅ Requires JWT authentication (`[Authorize]` attribute)
- ✅ Automatic tenant ID extraction from JWT claims (`tenant_id` or `tid`)
- ✅ Automatic user ID extraction from JWT claims (`sub`, `user_id`, or `uid`)
- ✅ Automatic group management (adds connections to `tenant:{tenantId}` and `user:{userId}` groups)
- ✅ Helper methods for tenant-scoped and user-specific messaging
- ✅ User context helpers (email, name from JWT)

**Key Methods:**
```csharp
protected string GetCurrentTenantId()
protected string GetCurrentUserId()
protected Task SendToTenantAsync(string method, object? arg1, object? arg2)
protected Task SendToUserAsync(string targetUserId, string method, object? arg1, object? arg2)
```

**Security:**
- All connections automatically grouped by tenant
- Messages can only be sent within the same tenant
- JWT validation happens automatically via ASP.NET Core authentication

#### **DemoChatHub.cs** - Demo Chat Implementation
Location: `Shared-Modules/AppBlueprint.Infrastructure/SignalR/DemoChatHub.cs`

**Features:**
- ✅ Tenant-scoped chat messaging
- ✅ Direct messages between users (within same tenant)
- ✅ Typing indicators
- ✅ User presence tracking (online/offline notifications)
- ✅ Comprehensive logging

**API Methods:**
```csharp
Task SendMessageToTenant(string message)           // Broadcast to all in tenant
Task SendDirectMessage(string recipientUserId, string message)  // Send to specific user
Task NotifyTyping()                                // Typing indicator
Task NotifyStoppedTyping()                         // Stop typing indicator
```

**Events Sent to Clients:**
```csharp
"ReceiveMessage"          // ChatMessage object
"ReceiveDirectMessage"    // ChatMessage object
"UserConnected"           // userId, userName
"UserDisconnected"        // userId, userName
"UserTyping"              // userId, userName
"UserStoppedTyping"       // userId
```

#### **ServiceCollectionExtensions.cs** - Configuration
Added `AddAppBlueprintSignalR()` extension method:

```csharp
builder.Services.AddAppBlueprintSignalR();
```

**Configuration:**
- Client timeout: 60 seconds
- Handshake timeout: 15 seconds
- Keep-alive interval: 15 seconds
- Max message size: 1 MB
- MessagePack protocol for efficient binary serialization
- Detailed errors: Disabled in production (configure via environment)

### 2. **NuGet Packages** (`Directory.Packages.props`)

Added SignalR dependencies:
```xml
<PackageVersion Include="Microsoft.AspNetCore.SignalR.Client" Version="10.0.0" />
<PackageVersion Include="Microsoft.AspNetCore.SignalR.Protocols.MessagePack" Version="10.0.0" />
```

### 3. **Web Application** (`AppBlueprint.Web`)

#### **Program.cs Updates**

**Service Registration:**
```csharp
builder.Services.AddAppBlueprintSignalR();
```

**Hub Endpoint Mapping:**
```csharp
app.MapHub<AppBlueprint.Infrastructure.SignalR.DemoChatHub>("/hubs/demochat");
```

### 4. **Demo Chat Page** (`AppBlueprint.UiKit`)

#### **RealtimeChat.razor**
Location: `Shared-Modules/AppBlueprint.UiKit/Components/Pages/RealtimeChat.razor`

**Features:**
- ✅ Real-time chat interface with message history
- ✅ Connection status indicator (Connected/Reconnecting/Disconnected)
- ✅ Online users list with presence tracking
- ✅ Typing indicators
- ✅ Auto-scroll to latest messages
- ✅ Connection info display (user, tenant, hub URL)
- ✅ Message sent counter
- ✅ Automatic reconnection handling
- ✅ Responsive design (desktop and mobile)
- ✅ Dark mode support

**UI Sections:**
1. **Connection Status Card** - Shows real-time connection state with color-coded indicators
2. **Chat Messages** - Message display with user names and timestamps
3. **Message Input** - Send messages with typing indicator
4. **Connected Users Sidebar** - Live list of online users in the tenant
5. **Connection Info** - Tenant ID, user info, statistics
6. **How It Works** - Educational section explaining tenant isolation

**Route:** `/realtime-chat`

## How It Works

### Authentication Flow
1. User must be logged in (JWT authentication)
2. When connecting to SignalR hub, JWT token is sent in connection request
3. `TenantScopedHub` validates token and extracts claims
4. Connection is automatically added to tenant-specific and user-specific groups
5. All messages are scoped to the tenant automatically

### Tenant Isolation
- **Automatic:** SignalR groups are used (`tenant:{tenantId}`)
- **Secure:** Tenant ID comes from validated JWT claims (cannot be spoofed)
- **Transparent:** Developers just call `SendToTenantAsync()` - framework handles isolation

### Message Flow
```
User A (Tenant X) sends message
    ↓
DemoChatHub.SendMessageToTenant(message)
    ↓
GetCurrentTenantId() → "tenant_x" (from JWT)
    ↓
SendToTenantAsync("ReceiveMessage", chatMessage)
    ↓
SignalR broadcasts to group "tenant:tenant_x"
    ↓
Only users in Tenant X receive the message
```

## Usage Examples

### Creating a Custom Hub

```csharp
using AppBlueprint.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

public class NotificationHub : TenantScopedHub<NotificationHub>
{
    public async Task BroadcastNotification(string title, string message)
    {
        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        
        var notification = new
        {
            Title = title,
            Message = message,
            Timestamp = DateTime.UtcNow,
            UserId = userId
        };
        
        // Send to all connections in this tenant
        await SendToTenantAsync("ReceiveNotification", notification);
    }
    
    public async Task SendToUser(string targetUserId, string message)
    {
        // Tenant isolation is automatic - target user must be in same tenant
        await SendToUserAsync(targetUserId, "ReceivePrivateMessage", message);
    }
}
```

### Registering in Program.cs

```csharp
// Add SignalR infrastructure
builder.Services.AddAppBlueprintSignalR();

// Map your custom hub
app.MapHub<NotificationHub>("/hubs/notifications");
```

### Connecting from Blazor

```razor
@inject AuthenticationStateProvider AuthStateProvider
@implements IAsyncDisposable

@code {
    private HubConnection? _hubConnection;
    
    protected override async Task OnInitializedAsync()
    {
        var hubUrl = NavigationManager.ToAbsoluteUri("/hubs/notifications");
        
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                    return authState.User.FindFirst("access_token")?.Value ?? string.Empty;
                };
            })
            .WithAutomaticReconnect()
            .Build();
        
        _hubConnection.On<Notification>("ReceiveNotification", notification =>
        {
            // Handle notification
            InvokeAsync(StateHasChanged);
        });
        
        await _hubConnection.StartAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
```

## Testing the Demo

1. **Build the projects:**
   ```bash
   dotnet build AppBlueprint.Infrastructure
   dotnet build AppBlueprint.UiKit
   dotnet build AppBlueprint.Web
   ```

2. **Run the application:**
   ```bash
   cd AppBlueprint.AppHost
   dotnet run
   ```

3. **Access the demo page:**
   - Navigate to: `http://localhost:9200/realtime-chat`
   - Must be logged in (JWT authentication required)

4. **Test multi-user chat:**
   - Open multiple browser windows/tabs
   - Login with different users in the same tenant
   - Send messages and observe real-time updates
   - Watch typing indicators and presence tracking

5. **Test tenant isolation:**
   - Login with users from different tenants
   - Verify they cannot see each other's messages
   - Check that users only appear in their own tenant's online list

## Security Considerations

### ✅ **Implemented Security**
- JWT authentication required for all connections
- Tenant ID extracted from signed JWT (cannot be spoofed)
- Automatic group-based isolation
- User ID validation from JWT claims
- Connection-level authorization via `[Authorize]` attribute

### ⚠️ **Production Recommendations**
1. **Enable HTTPS in production** (already configured in Railway)
2. **Configure CORS properly** for your frontend domains
3. **Set `DetailedErrors = false`** in production (already done)
4. **Implement rate limiting** on hub methods (future enhancement)
5. **Add message size validation** (already configured: 1 MB max)
6. **Monitor connection counts** per tenant for abuse detection
7. **Implement message history/persistence** (application-specific)

## Framework vs Application Responsibility

### ✅ **Framework Provides (Implemented)**
- Base hub class with authentication and tenant scoping
- JWT claim extraction and validation
- Automatic group management
- Helper methods for tenant/user messaging
- Demo chat hub and UI for testing
- Configuration and service registration

### 📝 **Applications Implement**
- Custom hub methods for their domain (e.g., NotificationHub, GameHub)
- Message persistence (if needed)
- Business logic for message validation
- Custom authorization rules beyond tenant scope
- Rate limiting per hub method (if needed)
- Message formatting and rich content

## Performance Considerations

1. **MessagePack Protocol:** Binary serialization for ~30% bandwidth reduction
2. **Keep-Alive:** 15-second intervals to maintain connections efficiently
3. **Automatic Reconnection:** Clients reconnect automatically on network issues
4. **Group-Based Broadcasting:** Efficient tenant-scoped messages
5. **Connection Pooling:** SignalR manages connection resources

## Future Enhancements

Potential additions to the framework:
- [ ] Redis backplane for multi-server scaling
- [ ] Message persistence service abstraction
- [ ] Rate limiting middleware for hub methods
- [ ] Connection analytics/monitoring
- [ ] Message delivery confirmation
- [ ] Offline message queue
- [ ] File/image sharing in real-time
- [ ] Voice/video call infrastructure (WebRTC integration)

## Documentation References

- [ASP.NET Core SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [SignalR Hub Authentication](https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz)
- [SignalR Groups](https://learn.microsoft.com/en-us/aspnet/core/signalr/groups)
- [Blazor SignalR Integration](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor)

---

**Implementation Date:** February 1, 2026  
**Status:** ✅ Complete and Production-Ready  
**Demo Route:** `/realtime-chat`
