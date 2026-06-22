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

// Cloudflare IP filter – blocks requests that do not originate from Cloudflare's
// published IP ranges or are missing the required CF-Ray / CF-Connecting-IP headers.
// Disabled automatically in Development; override via CloudflareFilter:Enabled.
app.UseMiddleware<CloudflareIpFilterMiddleware>();

// Security Headers Middleware - Add security headers to all responses
app.Use(async (context, next) =>
{
    // Strip technology-disclosure headers as late as possible so downstream middleware
    // and the proxy response pipeline don't reintroduce them before the response is sent.
    context.Response.OnStarting(static state =>
    {
        HttpResponse response = (HttpResponse)state;
        response.Headers.Remove("Server");
        response.Headers.Remove("X-Powered-By");
        response.Headers.Remove("X-AspNet-Version");

        return Task.CompletedTask;
    }, context.Response);

    // Prevent MIME-sniffing attacks
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";

    // Prevent clickjacking attacks by disallowing embedding in iframes
    context.Response.Headers["X-Frame-Options"] = "DENY";

    // Enable XSS protection in older browsers
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

    // Control referrer information sent with requests
    context.Response.Headers["Referrer-Policy"] = "no-referrer";

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

    context.Response.Headers["Content-Security-Policy"] = csp;

    // Restrict cross-domain policy files (Flash, PDF, etc.)
    context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";

    // Permissions Policy - control browser features
    context.Response.Headers["Permissions-Policy"] =
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

    // HSTS - only in production (Railway terminates SSL at the proxy level, browser enforces HTTPS)
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
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
