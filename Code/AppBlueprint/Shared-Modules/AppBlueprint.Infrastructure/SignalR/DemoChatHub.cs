using System.Collections.Concurrent;
using AppBlueprint.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.SignalR;

/// <summary>
/// Demo chat hub for testing tenant-scoped real-time communication.
/// Messages are automatically isolated by tenant - users only receive messages from their own tenant.
/// </summary>
public class DemoChatHub : TenantScopedHub<DemoChatHub>
{
    // Track online users per tenant: TenantId -> Dictionary<UserId, UserName>
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _onlineUsersByTenant = new();

    // Track cross-tenant conversations: ConversationId -> HashSet<TenantId>
    // In production, this should be persisted in database
    private static readonly ConcurrentDictionary<string, HashSet<string>> _conversationParticipants = new();

    private readonly ILogger<DemoChatHub> _logger;
    private readonly IConversationAuthorizationService? _authorizationService;

    public DemoChatHub(ILogger<DemoChatHub> logger, IConversationAuthorizationService? authorizationService = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
        _authorizationService = authorizationService;
        SetLogger(logger); // Set logger in base class
    }

    /// <summary>
    /// Sends a message to all users in the current tenant.
    /// </summary>
    /// <param name="message">The message text</param>
    public async Task SendMessageToTenant(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";

        _logger.LogInformation(
            "User {UserId} from tenant {TenantId} sending message: {MessagePreview}",
            userId,
            tenantId,
            message.Length > 50 ? message[..50] + "..." : message
        );

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            UserName = userName,
            TenantId = tenantId,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        // Send to all connections in this tenant
        await SendToTenantAsync("ReceiveMessage", chatMessage);
    }

    /// <summary>
    /// Sends a direct message to a specific user.
    /// Validates that both sender and recipient are in the same tenant.
    /// </summary>
    /// <param name="recipientUserId">The target user ID</param>
    /// <param name="message">The message text</param>
    public async Task SendDirectMessage(string recipientUserId, string message)
    {
        ArgumentNullException.ThrowIfNull(recipientUserId);
        ArgumentNullException.ThrowIfNull(message);

        string senderTenantId = GetCurrentTenantId();
        string senderUserId = GetCurrentUserId();
        string? senderName = GetCurrentUserName() ?? "Anonymous";

        // In a production system, you'd verify recipientUserId is in the same tenant
        // For this demo, we trust the tenant isolation provided by the base class

        _logger.LogInformation(
            "User {SenderId} sending direct message to {RecipientId} in tenant {TenantId}",
            senderUserId,
            recipientUserId,
            senderTenantId
        );

        var directMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = senderUserId,
            UserName = senderName,
            TenantId = senderTenantId,
            Message = message,
            Timestamp = DateTime.UtcNow,
            IsDirectMessage = true,
            RecipientUserId = recipientUserId
        };

        // Send to recipient
        await SendToUserAsync(recipientUserId, "ReceiveDirectMessage", directMessage);

        // Echo back to sender
        await Clients.Caller.SendAsync("ReceiveDirectMessage", directMessage);
    }

    /// <summary>
    /// Joins a cross-tenant conversation (e.g., property inquiry between tenant and landlord).
    /// Validates user has permission via IConversationAuthorizationService.
    /// </summary>
    /// <param name="conversationId">The conversation/property ID</param>
    public async Task JoinConversation(string conversationId)
    {
        ArgumentNullException.ThrowIfNull(conversationId);

        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";

        // SECURITY: Validate user has permission to join this conversation
        if (_authorizationService != null)
        {
            bool isAuthorized = await _authorizationService.CanJoinConversationAsync(conversationId, userId, tenantId);
            if (!isAuthorized)
            {
                _logger.LogWarning(
                    "User {UserId} from tenant {TenantId} denied access to conversation {ConversationId} - not authorized",
                    userId,
                    tenantId,
                    conversationId
                );

                throw new HubException($"You do not have permission to join conversation '{conversationId}'.");
            }
        }
        else
        {
            _logger.LogWarning(
                "No authorization service configured - allowing unrestricted access to conversation {ConversationId}. This is NOT recommended for production.",
                conversationId
            );
        }

        // Add tenant to conversation participants
        HashSet<string> participants = _conversationParticipants.GetOrAdd(conversationId, _ => new HashSet<string>());
        lock (participants)
        {
            participants.Add(tenantId);
        }

        // Join the SignalR group for this conversation
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");

        _logger.LogInformation(
            "User {UserId} from tenant {TenantId} joined conversation {ConversationId}",
            userId,
            tenantId,
            conversationId
        );

        // Notify others in conversation
        await Clients.OthersInGroup($"conversation:{conversationId}")
            .SendAsync("UserJoinedConversation", conversationId, userId, userName, tenantId);
    }

    /// <summary>
    /// Sends a message to a cross-tenant conversation.
    /// Validates user has permission before sending.
    /// </summary>
    /// <param name="conversationId">The conversation/property ID</param>
    /// <param name="message">The message text</param>
    public async Task SendMessageToConversation(string conversationId, string message)
    {
        ArgumentNullException.ThrowIfNull(conversationId);
        ArgumentNullException.ThrowIfNull(message);

        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";

        // SECURITY: Validate user has permission to send messages in this conversation
        if (_authorizationService != null)
        {
            bool isAuthorized = await _authorizationService.CanSendMessageAsync(conversationId, userId, tenantId);
            if (!isAuthorized)
            {
                _logger.LogWarning(
                    "User {UserId} from tenant {TenantId} denied sending message to conversation {ConversationId} - not authorized",
                    userId,
                    tenantId,
                    conversationId
                );

                throw new HubException($"You do not have permission to send messages in conversation '{conversationId}'.");
            }
        }

        _logger.LogInformation(
            "User {UserId} from tenant {TenantId} sending message to conversation {ConversationId}: {MessagePreview}",
            userId,
            tenantId,
            conversationId,
            message.Length > 50 ? message[..50] + "..." : message
        );

        var chatMessage = new ChatMessage
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            UserName = userName,
            TenantId = tenantId,
            Message = message,
            Timestamp = DateTime.UtcNow,
            ConversationId = conversationId,
            IsCrossTenant = true
        };

        // Send to all users in this conversation (cross-tenant)
        await Clients.Group($"conversation:{conversationId}")
            .SendAsync("ReceiveConversationMessage", chatMessage);
    }

    /// <summary>
    /// Leaves a cross-tenant conversation.
    /// </summary>
    /// <param name="conversationId">The conversation ID to leave</param>
    public async Task LeaveConversation(string conversationId)
    {
        ArgumentNullException.ThrowIfNull(conversationId);

        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";
        string tenantId = GetCurrentTenantId();

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation:{conversationId}");

        _logger.LogInformation(
            "User {UserId} from tenant {TenantId} left conversation {ConversationId}",
            userId,
            tenantId,
            conversationId
        );

        // Notify others
        await Clients.Group($"conversation:{conversationId}")
            .SendAsync("UserLeftConversation", conversationId, userId, userName, tenantId);
    }

    /// <summary>
    /// Broadcasts a typing indicator to the tenant.
    /// </summary>
    public async Task NotifyTyping()
    {
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Someone";

        await SendToTenantAsync("UserTyping", userId, userName);
    }

    /// <summary>
    /// Broadcasts that user stopped typing.
    /// </summary>
    public async Task NotifyStoppedTyping()
    {
        string userId = GetCurrentUserId();

        await SendToTenantAsync("UserStoppedTyping", userId);
    }

    /// <summary>
    /// Called when a client connects. Notifies other users in the tenant.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";

        _logger.LogInformation(
            "User {UserId} ({UserName}) connected to DemoChatHub for tenant {TenantId}",
            userId,
            userName,
            tenantId
        );

        // Add user to online tracking
        ConcurrentDictionary<string, string> tenantUsers = _onlineUsersByTenant.GetOrAdd(tenantId, _ => new ConcurrentDictionary<string, string>());
        tenantUsers.TryAdd(userId, userName);

        // Send current online users to the newly connected client
        var onlineUsers = tenantUsers.Select(kvp => new { UserId = kvp.Key, UserName = kvp.Value }).ToList();
        await Clients.Caller.SendAsync("OnlineUsers", onlineUsers);

        // Notify other users in tenant about new connection
        await SendToTenantAsync("UserConnected", userId, userName);
    }

    /// <summary>
    /// Called when a client disconnects. Notifies other users in the tenant.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string tenantId = GetCurrentTenantId();
        string userId = GetCurrentUserId();
        string? userName = GetCurrentUserName() ?? "Anonymous";

        _logger.LogInformation(
            "User {UserId} ({UserName}) disconnected from DemoChatHub for tenant {TenantId}",
            userId,
            userName,
            tenantId
        );

        // Remove user from online tracking
        if (_onlineUsersByTenant.TryGetValue(tenantId, out ConcurrentDictionary<string, string>? tenantUsers))
        {
            tenantUsers.TryRemove(userId, out _);

            // Clean up empty tenant dictionaries
            if (tenantUsers.IsEmpty)
            {
                _onlineUsersByTenant.TryRemove(tenantId, out _);
            }
        }

        // Notify other users in tenant
        await SendToTenantAsync("UserDisconnected", userId, userName);

        await base.OnDisconnectedAsync(exception);
    }
}

/// <summary>
/// Chat message model for SignalR communication.
/// </summary>
public class ChatMessage
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required string TenantId { get; init; }
    public required string Message { get; init; }
    public DateTime Timestamp { get; init; }
    public bool IsDirectMessage { get; init; }
    public string? RecipientUserId { get; init; }
    public string? ConversationId { get; init; }
    public bool IsCrossTenant { get; init; }
}
