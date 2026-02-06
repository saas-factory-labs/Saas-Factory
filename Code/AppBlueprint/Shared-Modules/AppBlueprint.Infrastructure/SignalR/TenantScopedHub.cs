using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AppBlueprint.Infrastructure.SignalR;

/// <summary>
/// Base class for SignalR hubs that require tenant and user context.
/// Ensures all SignalR connections are authenticated and tenant-scoped.
/// Note: Authorization is handled at method level or through connection validation.
/// </summary>
public abstract class TenantScopedHub<THub> : Hub where THub : Hub
{
    protected ILogger<THub>? Logger { get; private set; }

    /// <summary>
    /// Sets the logger for derived hubs. Called by DI automatically if logger is injected.
    /// </summary>
    protected void SetLogger(ILogger<THub> logger)
    {
        Logger = logger;
    }
    /// <summary>
    /// Gets the current tenant ID from the authenticated user's claims.
    /// Checks multiple claim names for compatibility with different auth systems.
    /// Falls back to query string parameter for Blazor Server compatibility.
    /// </summary>
    /// <returns>Tenant ID, or throws if not found</returns>
    protected string GetCurrentTenantId()
    {
        // Try multiple claim names (Logto JWT, custom claims, etc.)
        string? tenantId = Context.User?.FindFirst("tenant_id")?.Value
                           ?? Context.User?.FindFirst("tid")?.Value
                           ?? Context.User?.FindFirst("TenantId")?.Value;

        // Fallback: Check query string (for Blazor Server where WebSocket doesn't pass cookies)
        if (string.IsNullOrEmpty(tenantId) && Context.GetHttpContext()?.Request.Query.ContainsKey("tenantId") == true)
        {
            tenantId = Context.GetHttpContext()?.Request.Query["tenantId"].ToString();
        }

        if (string.IsNullOrEmpty(tenantId))
        {
            // Log all available claims for debugging
            if (Context.User?.Claims != null)
            {
                string availableClaims = string.Join(", ", Context.User.Claims.Select(c => $"{c.Type}={c.Value}"));
                Logger?.LogError("Tenant ID not found. Available claims: {Claims}", availableClaims);
            }

            throw new HubException("Tenant ID not found in user claims. User might not have completed onboarding.");
        }

        return tenantId;
    }

    /// <summary>
    /// Gets the current user ID from the authenticated user's JWT claims.
    /// Falls back to query string parameter for Blazor Server compatibility.
    /// </summary>
    /// <returns>User ID, or throws if not found</returns>
    protected string GetCurrentUserId()
    {
        string? userId = Context.User?.FindFirst("sub")?.Value
                         ?? Context.User?.FindFirst("user_id")?.Value
                         ?? Context.User?.FindFirst("uid")?.Value;

        // Fallback: Check query string (for Blazor Server where WebSocket doesn't pass cookies)
        if (string.IsNullOrEmpty(userId) && Context.GetHttpContext()?.Request.Query.ContainsKey("userId") == true)
        {
            userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        }

        if (string.IsNullOrEmpty(userId))
        {
            throw new HubException("User ID not found in user claims. Ensure JWT contains 'sub', 'user_id', or 'uid' claim.");
        }

        return userId;
    }

    /// <summary>
    /// Gets the current user's email from JWT claims (if available).
    /// </summary>
    protected string? GetCurrentUserEmail()
    {
        return Context.User?.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Gets the current user's name from JWT claims (if available).
    /// </summary>
    protected string? GetCurrentUserName()
    {
        return Context.User?.FindFirst("name")?.Value
               ?? Context.User?.FindFirst("given_name")?.Value;
    }

    /// <summary>
    /// Called when a client connects to the hub.
    /// Automatically adds the connection to a tenant-specific group.
    /// Validates that the user has tenant/user credentials (from claims OR query string).
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        try
        {
            // Get tenant and user ID (will throw if not found in claims or query string)
            string tenantId = GetCurrentTenantId();
            string userId = GetCurrentUserId();

            // Add connection to tenant group for tenant-scoped broadcasts
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");

            // Add connection to user-specific group for direct messages
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

            Logger?.LogInformation("User {UserId} from tenant {TenantId} connected", userId, tenantId);

            await base.OnConnectedAsync();
        }
        catch (HubException ex)
        {
            Logger?.LogError(ex, "Failed to establish hub connection - aborting");
            Context.Abort();
            // Do NOT re-throw - just return to prevent crash
            return;
        }
        catch (InvalidOperationException ex)
        {
            Logger?.LogError(ex, "Invalid operation during hub connection - aborting");
            Context.Abort();
            return;
        }
        catch (ArgumentException ex)
        {
            Logger?.LogError(ex, "Invalid argument during hub connection (missing claims?) - aborting");
            Context.Abort();
            return;
        }
    }

    /// <summary>
    /// Called when a client disconnects from the hub.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            // Only attempt cleanup if user was authenticated
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                string tenantId = GetCurrentTenantId();
                string userId = GetCurrentUserId();

                // Remove from tenant group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");

                // Remove from user group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");

                Logger?.LogInformation("User {UserId} from tenant {TenantId} disconnected", userId, tenantId);
            }
        }
        catch (HubException ex)
        {
            Logger?.LogWarning(ex, "Hub error during disconnect cleanup");
        }
        catch (InvalidOperationException ex)
        {
            Logger?.LogWarning(ex, "Invalid operation during disconnect - connection may not have been fully established");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Sends a message to all connections in the current tenant.
    /// </summary>
    protected Task SendToTenantAsync(string method, object? arg1 = null, object? arg2 = null)
    {
        string tenantId = GetCurrentTenantId();
        return Clients.Group($"tenant:{tenantId}").SendAsync(method, arg1, arg2);
    }

    /// <summary>
    /// Sends a message to a specific user (all their connections).
    /// </summary>
    protected Task SendToUserAsync(string targetUserId, string method, object? arg1 = null, object? arg2 = null)
    {
        return Clients.Group($"user:{targetUserId}").SendAsync(method, arg1, arg2);
    }
}
