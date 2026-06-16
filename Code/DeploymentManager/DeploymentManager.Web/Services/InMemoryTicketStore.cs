using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace DeploymentManager.Web.Services;

// Stores the full OIDC authentication ticket (access + refresh + ID tokens + all claims)
// server-side in the memory cache. The browser cookie becomes a small GUID key instead of
// a multi-chunk header that trips the HTTP 431 limit on large Logto token payloads.
//
// Trade-off: tickets are lost on container restart (users must re-authenticate). Acceptable
// for a single-instance internal tool. Switch to a Redis-backed store for multi-instance.
public sealed class InMemoryTicketStore(IMemoryCache cache) : ITicketStore
{
    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        string key = Guid.NewGuid().ToString("N");
        RenewAsync(key, ticket);
        return Task.FromResult(key);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        DateTimeOffset expiry = ticket.Properties.ExpiresUtc ?? DateTimeOffset.UtcNow.AddHours(12);
        cache.Set(key, ticket, expiry);
        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        cache.TryGetValue(key, out AuthenticationTicket? ticket);
        return Task.FromResult(ticket);
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }
}
