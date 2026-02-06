using AppBlueprint.AppGateway;
using AppBlueprint.AppGateway.Resources;
using AppBlueprint.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;

// Configure telemetry - must come before CreateBuilder and AddServiceDefaults
// Get the OTLP endpoint from environment or use Aspire default
const string dashboardEndpoint = "https://localhost:21250";
string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
const string otlpDefaultEndpoint = "http://localhost:18889";

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


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



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
