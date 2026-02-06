namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service for authorizing cross-tenant conversation access.
/// Implement this interface to control who can join/message in conversations.
/// </summary>
public interface IConversationAuthorizationService
{
    /// <summary>
    /// Checks if a user has permission to join a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation ID (e.g., property-123, match-456)</param>
    /// <param name="userId">The user attempting to join</param>
    /// <param name="tenantId">The user's tenant ID</param>
    /// <returns>True if authorized, false otherwise</returns>
    Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId);

    /// <summary>
    /// Checks if a user has permission to send messages in a conversation.
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="userId">The user attempting to send</param>
    /// <param name="tenantId">The user's tenant ID</param>
    /// <returns>True if authorized, false otherwise</returns>
    Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId);

    /// <summary>
    /// Gets all conversation IDs that a user has access to.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="tenantId">The user's tenant ID</param>
    /// <returns>List of conversation IDs</returns>
    Task<List<string>> GetUserConversationsAsync(string userId, string tenantId);
}
