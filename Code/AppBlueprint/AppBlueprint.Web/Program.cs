using AppBlueprint.Api.Client.Sdk;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Models;
using AppBlueprint.Web;
using AppBlueprint.Web.Components;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using MudBlazor.Services;
using _Imports = AppBlueprint.UiKit._Imports;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Configure telemetry - must come before AddServiceDefaults
// In Production/Railway, disable OTLP to prevent connection errors
// In Development, use Aspire Dashboard
if (builder.Environment.IsDevelopment())
{
    // Development mode - configure OTLP for Aspire Dashboard
    string? dashboardEndpoint = Environment.GetEnvironmentVariable("DOTNET_DASHBOARD_OTLP_ENDPOINT_URL");
    string? otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
    string otlpDefaultEndpoint = "http://localhost:18889";

    // Set OTLP endpoint with priority: DOTNET_DASHBOARD_OTLP_ENDPOINT_URL > OTEL_EXPORTER_OTLP_ENDPOINT > default
    if (!string.IsNullOrEmpty(dashboardEndpoint))
    {
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", dashboardEndpoint);
        Console.WriteLine($"[Web] Using dashboard OTLP endpoint: {dashboardEndpoint}");
    }
    else if (string.IsNullOrEmpty(otlpEndpoint))
    {
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", otlpDefaultEndpoint);
        Console.WriteLine($"[Web] Using default OTLP endpoint: {otlpDefaultEndpoint}");
    }
    else
    {
        Console.WriteLine($"[Web] Using existing OTLP endpoint: {otlpEndpoint}");
    }

    // Set OTLP protocol if not already set
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")))
    {
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
    }

    Console.WriteLine($"[Web] Final OTLP endpoint → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")}");
    Console.WriteLine($"[Web] Final OTLP protocol → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")}");
}
else
{
    // Production mode (Railway) - disable OTLP export to prevent connection errors
    // OpenTelemetry will still collect metrics/traces but won't export them
    // Set to empty/null to disable OTLP exporter
    Console.WriteLine("[Web] Production mode - OTLP telemetry export disabled (no Aspire Dashboard)");
    
    // Only set if explicitly provided via environment variable (e.g., for external observability)
    string? explicitOtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
    if (!string.IsNullOrEmpty(explicitOtlpEndpoint))
    {
        Console.WriteLine($"[Web] Using explicit OTLP endpoint: {explicitOtlpEndpoint}");
    }
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Suppress OpenTelemetry verbose debug logs
builder.Logging.AddFilter("OpenTelemetry", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore.DataProtection", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore.Server.Kestrel", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);

builder.AddServiceDefaults();

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});

var navigationRoutes = builder.Configuration
    .GetSection("Navigation:Routes")
    .Get<List<NavLinkMetadata>>() ?? new List<NavLinkMetadata>();
builder.Services.AddSingleton(navigationRoutes);

// Only configure Kestrel ports for production (Railway)
// In development, Aspire AppHost controls the ports
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Production (Railway): Use port 80
        options.ListenAnyIP(80);
        Console.WriteLine("[Web] Production mode - HTTP (80), HTTPS handled by Railway edge");
    });
}
else
{
    Console.WriteLine("[Web] Development mode - Ports controlled by Aspire AppHost");
}

builder.Services.ConfigureHttpClientDefaults(http =>
{
    if (builder.Environment.IsDevelopment())
    {
        http.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
    }
});

builder.Services.AddOutputCache();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState(); // Required for Blazor authentication
builder.Services.AddMudServices();
builder.Services.AddSingleton<BreadcrumbService>();
builder.Services.AddUiKit();

// Add HttpContextAccessor for accessing authentication tokens in delegating handlers
builder.Services.AddHttpContextAccessor();

// Configure authentication (Logto, cookies, data protection, authorization)
builder.Services.AddWebAuthentication(builder.Configuration, builder.Environment);

// Register Kiota IAuthenticationProvider for API client
// This provider works with ASP.NET Core's cookie-based authentication (Logto SDK)
// and optionally uses JWT tokens from storage for API calls
builder.Services.AddScoped<IAuthenticationProvider, 
    AppBlueprint.Infrastructure.Authorization.AspNetCoreKiotaAuthenticationProvider>();

// Get API base URL from environment variable or configuration
// Priority: Environment variable > Configuration > Default localhost
string apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") 
    ?? builder.Configuration["ApiBaseUrl"] 
    ?? "http://localhost:8091";

Console.WriteLine($"[Web] API Base URL configured: {apiBaseUrl}");

builder.Services.AddScoped<IRequestAdapter>(sp =>
    new HttpClientRequestAdapter(sp.GetRequiredService<IAuthenticationProvider>())
    {
        BaseUrl = apiBaseUrl
    });
builder.Services.AddScoped<ApiClient>(sp => new ApiClient(sp.GetRequiredService<IRequestAdapter>()));

// Register authentication handler for TodoService as Scoped (not Transient)
// Must be Scoped to work with ITokenStorageService which requires HttpContext
builder.Services.AddScoped<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add TodoService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TodoService>(client =>
{
    // Use API base URL from environment/configuration
    // Supports Railway deployment with API_BASE_URL environment variable
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        // Accept self-signed certificates in development
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add TeamService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TeamService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add RoleService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.RoleService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

var app = builder.Build();

Console.WriteLine("========================================");
Console.WriteLine("[Web] Application built successfully");
Console.WriteLine($"[Web] Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("========================================");

app.UseRouting();
// app.UseHttpsRedirection(); // Temporarily disabled for design review

// Security Headers Middleware - Add security headers to all responses
app.Use(async (context, next) =>
{
    // Prevent MIME-sniffing attacks
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Prevent clickjacking attacks by disallowing embedding in iframes
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Enable XSS protection in older browsers
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

    // Control referrer information sent with requests
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");

    // Content Security Policy - restrict resource loading to prevent XSS
    // Note: Adjust this policy based on your application's requirements
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://fonts.googleapis.com; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' https://32nkyp.logto.app wss://localhost:* ws://localhost:*;");

    // Permissions Policy - control browser features
    context.Response.Headers.Append("Permissions-Policy",
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    await next();
});

app.UseStaticFiles();
app.UseAntiforgery();

// Add authentication and authorization middleware for Logto
app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly)
    .AddInteractiveServerRenderMode();

// Logto authentication endpoints - EXACTLY as per documentation
// https://docs.logto.io/quick-starts/dotnet-core/blazor-server
app.MapAuthenticationEndpoints(builder.Configuration);

// Diagnostic endpoint to test Logto connectivity from Railway
app.MapLogtoTestEndpoint(builder.Configuration, app.Environment);

app.MapDefaultEndpoints();

Console.WriteLine("========================================");
Console.WriteLine("[Web] Starting application...");
Console.WriteLine("[Web] Navigate to the app and watch for logs");
Console.WriteLine("========================================");

app.Run();


