using AppBlueprint.AppGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;

// Configure telemetry - must come before CreateBuilder and AddServiceDefaults
// Get the OTLP endpoint from environment or use Aspire default
string? dashboardEndpoint = "https://localhost:21250"; //Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
string otlpDefaultEndpoint = "http://localhost:18889";

// Set OTLP endpoint with priority: DOTNET_DASHBOARD_OTLP_ENDPOINT_URL > OTEL_EXPORTER_OTLP_ENDPOINT > default
if (!string.IsNullOrEmpty(dashboardEndpoint))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
    Console.WriteLine($"[AppGateway] Using dashboard OTLP endpoint: {dashboardEndpoint}");
}
else if (string.IsNullOrEmpty(otlpEndpoint))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", otlpDefaultEndpoint);
    Console.WriteLine($"[AppGateway] Using default OTLP endpoint: {otlpDefaultEndpoint}");
}
else
{
    Console.WriteLine($"[AppGateway] Using existing OTLP endpoint: {otlpEndpoint}");
}

// Set OTLP protocol if not already set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")))
{
    Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
}

// Print final configuration
Console.WriteLine($"[AppGateway] Final OTLP endpoint → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")}");
Console.WriteLine($"[AppGateway] Final OTLP protocol → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")}");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add default services (e.g., logging, configuration, DI)
builder.AddServiceDefaults();

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

// Add OpenAPI/Swagger support for API documentation.
// builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

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
            cancellationToken).ConfigureAwait(false);
    };
});

// Add global rate limiter configuration.
builder.Services.AddRateLimiter(RateLimiterConfig.Configure);


/*
 * TODO: implement these features:
 *  Load balancing
    Response caching
    Monitoring
    tjek platform platform for yarp proxy setup
    limit the downstream microservices to only accept traffic from the api gateway for security purposes
 *
 */

// TODO: implement loading proxy routes from remote config like or implement such that routes are automatically sourced directly from the api and frontend projects by calling a minimal api endpoint on the frontend and api service to always keep in sync: 
/*
 * Alternative	Cloud / Self-Hosted	Real-Time Updates	Good for Large Scale	Complexity
    Azure App Config	Cloud	✔	✔	🔹🔹
    Consul	Both	✔	✔✔	🔹🔹🔹
    etcd	Both	✔	✔✔✔	🔹🔹🔹
    AWS AppConfig	Cloud	✔	✔	🔹🔹
    Google Runtime Config	Cloud	✔	✔	🔹🔹
    PostgreSQL/SQL	Self-Hosted	❌	✔	🔹
    Redis	Both	✔✔	✔✔	🔹🔹
    ZooKeeper	Self-Hosted	✔	✔✔✔	🔹🔹🔹
    GitOps (Git)	Both	✔ (Webhook)	✔	🔹🔹
    File-Based (S3, Blob)	Both	❌ (Polling)	✔	🔹
 */


// Configure Reverse Proxy from appsettings configuration.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoints in development mode.
    // app.MapOpenApi();
}

// app.UseHttpsRedirection();

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
