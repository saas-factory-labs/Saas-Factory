using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace AppBlueprint.AppGateway;

internal static class RateLimiterConfig
{
    public static void Configure(RateLimiterOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Set a global rate limiter based on Fixed Window Rate Limiting.
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            // SECURITY: partition by client IP, not the Host header. The Host header is fully
            // client-controlled, so partitioning on it lets an attacker reset their quota at will.
            string partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                partition => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5, // Maximum 5 requests allowed.
                    Window = TimeSpan.FromSeconds(10), // Time window for rate limiting.
                    AutoReplenishment = true,
                    QueueLimit = 5, // Allow up to 5 requests to queue.
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                });
        });

        // Handle rejected requests with a 429 status code and custom message.
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = 429;
            await context.HttpContext.Response.WriteAsync("Too many requests.", token);
        };
    }
}
