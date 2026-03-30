using AppBlueprint.AppGateway;
using AppBlueprint.AppGateway.Resources;
using AppBlueprint.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;

// Configure telemetry - must come before CreateBuilder and AddServiceDefaults
// Use environment variable from AppHost; fall back to Aspire default only if unset
string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
const string otlpDefaultEndpoint = "http://localhost:21250";

// Set OTLP endpoint with priority: DOTNET_DASHBOARD_OTLP_ENDPOINT_URL > OTEL_EXPORTER_OTLP_ENDPOINT > default
#pragma warning disable CA1303 // Do not pass literals as localized parameters - using resource constants
if (!string.IsNullOrEmpty(dashboardEndpoint))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
    Console.WriteLine(ConfigurationMessages.DashboardEndpointMessage, dashboardEndpoint);
}
else if (string.IsNullOrEmpty(otlpEndpoint))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", otlpDefaultEndpoint);
    Console.WriteLine(ConfigurationMessages.DefaultEndpointMessage, otlpDefaultEndpoint);
}
else
{
    Console.WriteLine(ConfigurationMessages.ExistingEndpointMessage, otlpEndpoint);
}
#pragma warning restore CA1303

// Set OTLP protocol if not already set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
}

// Print final configuration
#pragma warning disable CA1303 // Do not pass literals as localized parameters - using resource constants
Console.WriteLine(ConfigurationMessages.FinalOtlpEndpointMessage, Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));
Console.WriteLine(ConfigurationMessages.FinalOtlpProtocolMessage, Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"));
#pragma warning restore CA1303

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add default services (e.g., logging, configuration, DI)
builder.AddServiceDefaults();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("authenticated", policy =>
        policy.RequireAuthenticatedUser());

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });

    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Try again later.",
            cancellationToken);
    };
});

// Add global rate limiter configuration.
builder.Services.AddRateLimiter(RateLimiterConfig.Configure);

// Configure Reverse Proxy from appsettings configuration.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Suppress the "Server: Kestrel" response header to avoid leaking server technology details.
builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security Headers Middleware - Add security headers to all responses
app.Use(async (context, next) =>
{
    // Remove real headers and replace with deceptive ones to mislead attackers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Remove("X-AspNet-Version");

    // Deception headers - mislead attackers with false server technology information
    context.Response.Headers.Append("Server", "Apache/2.4.62 (Ubuntu)");
    context.Response.Headers.Append("X-Powered-By", "PHP/8.2.28");

    // Prevent MIME-sniffing attacks
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Prevent clickjacking attacks by disallowing embedding in iframes
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Enable XSS protection in older browsers
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Control referrer information sent with requests
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");

    // Content Security Policy.
    // In development the gateway serves Swagger UI which needs 'unsafe-inline' for its bundled scripts.
    // In production the gateway is a pure reverse proxy with no browser-facing HTML, so enforce a
    // strict policy. 'unsafe-eval' is never required.
    string csp = app.Environment.IsDevelopment()
        ? "default-src 'self'; " +
          "script-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://www.gstatic.com; " +
          "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://www.gstatic.com; " +
          "font-src 'self' https://fonts.gstatic.com; " +
          "img-src 'self' data: https:; " +
          "connect-src 'self' https://32nkyp.logto.app wss: ws: https://cdn.jsdelivr.net https://www.gstatic.com https://*.firebaseio.com https://*.googleapis.com; " +
          "frame-ancestors 'none';"
        : "default-src 'none'; frame-ancestors 'none';";

    context.Response.Headers.Append("Content-Security-Policy", csp);

    // Restrict cross-domain policy files (Flash, PDF, etc.)
    context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

    // Permissions Policy - control browser features
    context.Response.Headers.Append("Permissions-Policy",
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    // HSTS - only in production (Railway terminates SSL at the proxy level, browser enforces HTTPS)
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await next();
});


// Health check endpoint for monitoring.
app.MapGet("/health", () => Results.Ok("Healthy"));

// Map standard Aspire health checks (/healthz, etc.)
app.MapDefaultEndpoints();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

// Start the application.
app.Run();
