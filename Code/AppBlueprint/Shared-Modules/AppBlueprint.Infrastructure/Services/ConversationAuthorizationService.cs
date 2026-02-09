using AppBlueprint.Application.Interfaces;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Example implementation for dating app scenario.
/// Only matched users can communicate.
/// </summary>
public sealed class DatingAppConversationAuthorizationService : IConversationAuthorizationService
{
    // In production, inject your DbContext here
    // private readonly BaselineDbContext _dbContext;

    // public DatingAppConversationAuthorizationService(BaselineDbContext dbContext)
    // {
    //     _dbContext = dbContext;
    // }

    public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
    {
        ArgumentNullException.ThrowIfNull(conversationId);
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(tenantId);

        // Example: conversationId format = "match-{userId1}-{userId2}"
        // Extract user IDs and verify this user is one of them

        if (conversationId.StartsWith("match-", StringComparison.Ordinal))
        {
            string[] parts = conversationId.Split('-');
            if (parts.Length == 3)
            {
                string user1 = parts[1];
                string user2 = parts[2];

                // Check if current user is one of the matched users
                if (userId == user1 || userId == user2)
                {
                    // TODO: Query database to verify match exists and is active
                    // return await _dbContext.Matches
                    //     .AnyAsync(m => m.ConversationId == conversationId 
                    //                 && m.IsActive 
                    //                 && (m.User1Id == userId || m.User2Id == userId));

                    // For demo: allow if user ID is in conversation ID
                    return true;
                }
            }
        }

        return false;
    }

    public async Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId)
    {
        // Same logic as CanJoinConversationAsync
        // In some cases, you might want different rules (e.g., read-only access)
        return await CanJoinConversationAsync(conversationId, userId, tenantId);
    }

    public async Task<List<string>> GetUserConversationsAsync(string userId, string tenantId)
    {
        // TODO: Query database for all conversations this user has access to
        // return await _dbContext.Matches
        //     .Where(m => m.IsActive && (m.User1Id == userId || m.User2Id == userId))
        //     .Select(m => m.ConversationId)
        //     .ToListAsync();

        // For demo: return empty list
        return await Task.FromResult<List<string>>([]);
    }
}

/// <summary>
/// Example implementation for property rental scenario.
/// Property owner and interested tenants can communicate.
/// </summary>
public sealed class PropertyRentalConversationAuthorizationService : IConversationAuthorizationService
{
    // In production, inject your DbContext here

    public async Task<bool> CanJoinConversationAsync(string conversationId, string userId, string tenantId)
    {
        ArgumentNullException.ThrowIfNull(conversationId);
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(tenantId);

        // Example: conversationId format = "property-{propertyId}"
        // Check if user is property owner OR has sent an inquiry

        if (conversationId.StartsWith("property-", StringComparison.Ordinal))
        {
            string propertyId = conversationId.Replace("property-", "", StringComparison.Ordinal);

            // TODO: Query database
            // 1. Check if user owns the property
            // var property = await _dbContext.Properties.FindAsync(propertyId);
            // if (property?.OwnerId == userId) return true;
            //
            // 2. Check if user has submitted an inquiry
            // return await _dbContext.PropertyInquiries
            //     .AnyAsync(i => i.PropertyId == propertyId 
            //                 && i.InterestedUserId == userId 
            //                 && i.Status != InquiryStatus.Rejected);

            // For demo: allow all
            return true;
        }

        return false;
    }

    public async Task<bool> CanSendMessageAsync(string conversationId, string userId, string tenantId)
    {
        return await CanJoinConversationAsync(conversationId, userId, tenantId);
    }

    public async Task<List<string>> GetUserConversationsAsync(string userId, string tenantId)
    {
        // TODO: Query database for properties owned or inquired about
        return await Task.FromResult<List<string>>([]);
    }
}
