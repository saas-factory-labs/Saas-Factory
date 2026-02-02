# SignalR Conversation Authorization Guide

## Overview
This guide explains how to secure cross-tenant SignalR conversations with authorization logic to control who can join and send messages.

## Architecture

### Components
1. **`IConversationAuthorizationService`** (Application layer) - Interface defining authorization rules
2. **Implementation classes** (Infrastructure layer) - Business-specific authorization logic
3. **`DemoChatHub`** (Infrastructure layer) - SignalR hub with authorization checks
4. **Database tables** (Domain/Infrastructure) - Store conversation permissions

---

## Implementation Guide

### Step 1: Choose Your Scenario

**Dating App** - Only matched users can communicate:
```csharp
services.AddScoped<IConversationAuthorizationService, DatingAppConversationAuthorizationService>();
```

**Property Rental** - Property owners and interested tenants:
```csharp
services.AddScoped<IConversationAuthorizationService, PropertyRentalConversationAuthorizationService>();
```

**Custom Logic**:
```csharp
services.AddScoped<IConversationAuthorizationService, YourCustomAuthorizationService>();
```

### Step 2: Create Database Tables

Example for dating app:
```sql
CREATE TABLE matches (
    id UUID PRIMARY KEY,
    conversation_id VARCHAR(255) NOT NULL UNIQUE,
    user1_id VARCHAR(255) NOT NULL,
    user2_id VARCHAR(255) NOT NULL,
    tenant1_id VARCHAR(255) NOT NULL,
    tenant2_id VARCHAR(255) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    matched_at TIMESTAMP NOT NULL DEFAULT NOW(),
    INDEX idx_user1 (user1_id),
    INDEX idx_user2 (user2_id),
    INDEX idx_conversation (conversation_id)
);
```

Example for property rental:
```sql
CREATE TABLE property_inquiries (
    id UUID PRIMARY KEY,
    property_id VARCHAR(255) NOT NULL,
    conversation_id VARCHAR(255) NOT NULL,
    property_owner_id VARCHAR(255) NOT NULL,
    interested_user_id VARCHAR(255) NOT NULL,
    interested_user_tenant_id VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL, -- 'pending', 'accepted', 'rejected'
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    INDEX idx_property (property_id),
    INDEX idx_conversation (conversation_id),
    INDEX idx_interested_user (interested_user_id)
);
```

### Step 3: Implement Authorization Service

```csharp
public sealed class YourAuthorizationService : IConversationAuthorizationService
{
    private readonly BaselineDbContext _dbContext;
    private readonly ILogger<YourAuthorizationService> _logger;

    public YourAuthorizationService(BaselineDbContext dbContext, ILogger<YourAuthorizationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
    {
        // Example: Check database for permission
        bool hasPermission = await _dbContext.ConversationPermissions
            .AnyAsync(p => p.ConversationId == conversationId 
                        && p.UserId == userId 
                        && p.IsActive);

        if (!hasPermission)
        {
            _logger.LogWarning(
                "User {UserId} denied access to conversation {ConversationId}",
                userId,
                conversationId
            );
        }

        return hasPermission;
    }

    public async Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId)
    {
        // Same logic or different (e.g., read-only access)
        return await CanJoinConversationAsync(conversationId, userId, tenantId);
    }

    public async Task<List<string>> GetUserConversationsAsync(string userId, string tenantId)
    {
        return await _dbContext.ConversationPermissions
            .Where(p => p.UserId == userId && p.IsActive)
            .Select(p => p.ConversationId)
            .ToListAsync();
    }
}
```

### Step 4: Register Service in Program.cs

```csharp
// In your Web project's Program.cs
builder.Services.AddScoped<IConversationAuthorizationService, YourAuthorizationService>();
```

---

## How It Works

### When User Joins Conversation

1. Client calls: `await hubConnection.SendAsync("JoinConversation", "match-user1-user2")`
2. Hub extracts `userId` and `tenantId` from authenticated context
3. Hub calls: `await _authorizationService.CanJoinConversationAsync(conversationId, userId, tenantId)`
4. Service queries database to verify permission
5. **If authorized**: User joins SignalR group
6. **If not authorized**: `HubException` thrown → Client receives error

### When User Sends Message

1. Client calls: `await hubConnection.SendAsync("SendMessageToConversation", "match-user1-user2", "Hello!")`
2. Hub calls: `await _authorizationService.CanSendMessageAsync(conversationId, userId, tenantId)`
3. **If authorized**: Message broadcast to group
4. **If not authorized**: `HubException` thrown

---

## Security Best Practices

### 1. Always Validate on Server
❌ **Wrong** - Client-side validation only:
```javascript
// NEVER trust client-side checks
if (userIsAllowed) {
    await hubConnection.send("JoinConversation", conversationId);
}
```

✅ **Correct** - Server validates every request:
```csharp
public async Task JoinConversation(string conversationId)
{
    // Always check permission on server
    if (!await _authorizationService.CanJoinConversationAsync(...))
    {
        throw new HubException("Not authorized");
    }
}
```

### 2. Use Database-Backed Permissions
Don't rely on in-memory dictionaries - permissions must survive app restarts.

### 3. Log Authorization Failures
```csharp
_logger.LogWarning(
    "User {UserId} from tenant {TenantId} denied access to conversation {ConversationId}",
    userId, tenantId, conversationId
);
```

### 4. Implement Audit Trail
Track who accessed what conversation and when:
```csharp
await _dbContext.ConversationAccessLog.AddAsync(new ConversationAccessLog
{
    ConversationId = conversationId,
    UserId = userId,
    TenantId = tenantId,
    Action = "Join",
    AccessedAt = DateTime.UtcNow,
    WasAuthorized = isAuthorized
});
```

---

## Example Scenarios

### Dating App: Match-Based Access
```
Conversation ID: "match-alice-bob"
- Alice (tenant_A) can join ✅
- Bob (tenant_B) can join ✅
- Charlie (tenant_C) CANNOT join ❌
```

**Implementation:**
```csharp
public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
{
    if (!conversationId.StartsWith("match-", StringComparison.Ordinal))
        return false;

    return await _dbContext.Matches
        .AnyAsync(m => m.ConversationId == conversationId 
                    && m.IsActive
                    && (m.User1Id == userId || m.User2Id == userId));
}
```

### Property Rental: Owner + Inquirers
```
Conversation ID: "property-123"
- Property Owner can join ✅
- User who submitted inquiry can join ✅
- Random user CANNOT join ❌
```

**Implementation:**
```csharp
public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
{
    string propertyId = conversationId.Replace("property-", "", StringComparison.Ordinal);

    // Check if user owns the property
    Property? property = await _dbContext.Properties.FindAsync(propertyId);
    if (property?.OwnerId == userId)
        return true;

    // Check if user has submitted an inquiry
    return await _dbContext.PropertyInquiries
        .AnyAsync(i => i.PropertyId == propertyId 
                    && i.InterestedUserId == userId 
                    && i.Status != InquiryStatus.Rejected);
}
```

---

## Testing

### Manual Testing
1. Login as User A (Tenant 1)
2. Join conversation: "test-userA-userB"
3. Login as User B (Tenant 2) in incognito window
4. Join same conversation: "test-userA-userB"
5. **Both should connect** ✅
6. Login as User C (Tenant 3) in another window
7. Try to join: "test-userA-userB"
8. **Should fail with "Not authorized"** ✅

### Unit Test Example
```csharp
[Fact]
public async Task CanJoinConversation_WhenUserIsMatched_ReturnsTrue()
{
    // Arrange
    string conversationId = "match-user1-user2";
    string userId = "user1";
    string tenantId = "tenant1";
    
    // Add match to database
    await _dbContext.Matches.AddAsync(new Match
    {
        ConversationId = conversationId,
        User1Id = "user1",
        User2Id = "user2",
        IsActive = true
    });
    await _dbContext.SaveChangesAsync();
    
    // Act
    bool canJoin = await _authorizationService.CanJoinConversationAsync(conversationId, userId, tenantId);
    
    // Assert
    Assert.True(canJoin);
}

[Fact]
public async Task CanJoinConversation_WhenUserNotMatched_ReturnsFalse()
{
    // Arrange
    string conversationId = "match-user1-user2";
    string userId = "user3"; // Not in match
    string tenantId = "tenant3";
    
    // Act
    bool canJoin = await _authorizationService.CanJoinConversationAsync(conversationId, userId, tenantId);
    
    // Assert
    Assert.False(canJoin);
}
```

---

## Troubleshooting

### "You do not have permission to join conversation"
- Check database - does permission record exist?
- Check userId matches exactly (case-sensitive)
- Check `IsActive` flag is true
- Check authorization service is registered in DI

### Messages Not Appearing
- Ensure both users are in the same SignalR group
- Check browser console for JavaScript errors
- Verify `conversationId` is exactly the same for both users
- Check hub logs for authorization failures

---

## Production Checklist
- [ ] Authorization service registered in DI
- [ ] Database tables created with indexes
- [ ] Authorization failures logged
- [ ] Audit trail implemented
- [ ] Unit tests for authorization logic
- [ ] Integration tests for hub methods
- [ ] Performance tested with expected user load
- [ ] Rate limiting added to prevent abuse
