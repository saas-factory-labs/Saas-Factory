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
            // Partition by Host header.
            return RateLimitPartition.GetFixedWindowLimiter(
                httpContext.Request.Headers.Host.ToString(),
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
