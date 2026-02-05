using AppBlueprint.Api.Client.Sdk;
using AppBlueprint.Application.Extensions;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Models;
using AppBlueprint.Web;
using AppBlueprint.Web.Components;
using Blazored.LocalStorage;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using _Imports = AppBlueprint.UiKit._Imports;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

// Environment variable names
const string DotnetDashboardOtlpEndpointUrl = "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL";
const string OtelExporterOtlpEndpoint = "OTEL_EXPORTER_OTLP_ENDPOINT";

// Console output formatting
const string ConsoleSeparator = "========================================";
const string OtelExporterOtlpProtocol = "OTEL_EXPORTER_OTLP_PROTOCOL";
const string ApiBaseUrl = "API_BASE_URL";

var builder = WebApplication.CreateBuilder(args);

// Configure forwarded headers for Railway (reverse proxy)
// This ensures the app knows it's behind HTTPS even though it receives HTTP internally
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    Console.WriteLine("[Web] Configured to trust forwarded headers from Railway proxy");
});

// Configure telemetry - must come before AddServiceDefaults
// In Production/Railway, disable OTLP to prevent connection errors
// In Development, use Aspire Dashboard
if (builder.Environment.IsDevelopment())
{
    // Development mode - configure OTLP for Aspire Dashboard
    string? dashboardEndpoint = Environment.GetEnvironmentVariable(DotnetDashboardOtlpEndpointUrl);
    string? otlpEndpoint = Environment.GetEnvironmentVariable(OtelExporterOtlpEndpoint);
    string otlpDefaultEndpoint = "http://localhost:18889";

    // Set OTLP endpoint with priority: DOTNET_DASHBOARD_OTLP_ENDPOINT_URL > OTEL_EXPORTER_OTLP_ENDPOINT > default
    if (!string.IsNullOrEmpty(dashboardEndpoint))
    {
        Environment.SetEnvironmentVariable(OtelExporterOtlpEndpoint, dashboardEndpoint);
        Console.WriteLine($"[Web] Using dashboard OTLP endpoint: {dashboardEndpoint}");
    }
    else if (string.IsNullOrEmpty(otlpEndpoint))
    {
        Environment.SetEnvironmentVariable(OtelExporterOtlpEndpoint, otlpDefaultEndpoint);
        Console.WriteLine($"[Web] Using default OTLP endpoint: {otlpDefaultEndpoint}");
    }
    else
    {
        Console.WriteLine($"[Web] Using existing OTLP endpoint: {otlpEndpoint}");
    }

    // Set OTLP protocol if not already set
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(OtelExporterOtlpProtocol)))
    {
        Environment.SetEnvironmentVariable(OtelExporterOtlpProtocol, "http/protobuf");
    }

    Console.WriteLine($"[Web] Final OTLP endpoint → {Environment.GetEnvironmentVariable(OtelExporterOtlpEndpoint)}");
    Console.WriteLine($"[Web] Final OTLP protocol → {Environment.GetEnvironmentVariable(OtelExporterOtlpProtocol)}");
}
else
{
    // Production mode (Railway) - disable OTLP export to prevent connection errors
    // OpenTelemetry will still collect metrics/traces but won't export them
    // Set to empty/null to disable OTLP exporter
    Console.WriteLine("[Web] Production mode - OTLP telemetry export disabled (no Aspire Dashboard)");
    
    // Only set if explicitly provided via environment variable (e.g., for external observability)
    string? explicitOtlpEndpoint = Environment.GetEnvironmentVariable(OtelExporterOtlpEndpoint);
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

// Register configuration options with validation
builder.Services.AddAppBlueprintConfiguration(builder.Configuration, builder.Environment);

// Register infrastructure services (database contexts, repositories, etc.)
// Using Hybrid Mode to support both B2C and B2B user flows in the demo app
builder.Services.AddAppBlueprintInfrastructure(builder.Configuration, builder.Environment);

// Register application services (command handlers, validators, services)
builder.Services.AddAppBlueprintApplication();

var navigationRoutes = builder.Configuration
    .GetSection("Navigation:Routes")
    .Get<List<NavLinkMetadata>>() ?? new List<NavLinkMetadata>();
builder.Services.AddSingleton(navigationRoutes);

// Port configuration handled by ASPNETCORE_URLS environment variable:
// - Development: Set via launchSettings.json (http://localhost:5000;https://localhost:5001)
// - Production: Set via Dockerfile ENV ASPNETCORE_URLS=http://+:80 (Railway handles SSL termination)
// No ConfigureKestrel needed - avoids conflicts with environment variables

// Configure cookie policy for HTTP LAN access (fixes "Correlation failed" error)
// This allows authentication cookies to work over HTTP in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.MinimumSameSitePolicy = SameSiteMode.Lax;
        options.Secure = CookieSecurePolicy.None; // Allow cookies over HTTP in development
        options.CheckConsentNeeded = _ => false; // Disable consent requirement for essential auth cookies
    });
}

builder.Services.AddOutputCache();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Add API controllers for Web-specific endpoints (e.g., FirebaseConfigController)
builder.Services.AddControllers();

// Add minimal API versioning to register the 'apiVersion' route constraint
// This allows controllers from referenced assemblies to use versioned routes without errors
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
}).AddMvc();

// Configure Blazor Server Circuit Options for detailed error messages in development
builder.Services.AddServerSideBlazor(options =>
{
    // Enable detailed errors in development to help diagnose circuit failures
    options.DetailedErrors = builder.Environment.IsDevelopment();
});

builder.Services.AddCascadingAuthenticationState(); // Required for Blazor authentication

// Add Blazored LocalStorage for signup session persistence
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddUiKit();

// Add Email Template Service (RazorLight templating)
// Option: Enable custom templates by specifying path
// string customTemplatesPath = Path.Combine(builder.Environment.ContentRootPath, "Templates");
// builder.Services.AddEmailTemplateService(customTemplatesPath);
builder.Services.AddEmailTemplateService(); // Uses framework's embedded templates only

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
string apiBaseUrl = Environment.GetEnvironmentVariable(ApiBaseUrl) 
    ?? builder.Configuration["ApiBaseUrl"] 
    ?? "http://localhost:9100";

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

// Add SignalR with tenant-aware authentication
builder.Services.AddAppBlueprintSignalR();

// Register menu configuration service for customizing sidebar visibility
builder.Services.AddScoped<AppBlueprint.UiKit.Services.IMenuConfigurationService, AppBlueprint.Web.Services.MenuConfigurationService>();

// Register full-text search services
builder.Services.AddScoped<AppBlueprint.Application.Interfaces.ISearchService<AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant.TenantEntity>, 
    AppBlueprint.Infrastructure.Services.Search.PostgreSqlSearchService<AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant.TenantEntity, AppBlueprint.Infrastructure.DatabaseContexts.ApplicationDbContext>>();
builder.Services.AddScoped<AppBlueprint.Application.Interfaces.ISearchService<AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.UserEntity>, 
    AppBlueprint.Infrastructure.Services.Search.PostgreSqlSearchService<AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.UserEntity, AppBlueprint.Infrastructure.DatabaseContexts.ApplicationDbContext>>();

// Add TodoService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TodoService>(client =>
{
    // Use API base URL from environment/configuration
    // Supports Railway deployment with API_BASE_URL environment variable
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add TeamService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.TeamService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add RoleService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.RoleService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>();

// Add FileStorageService with HttpClient configured for direct API access
builder.Services.AddHttpClient<AppBlueprint.Web.Services.FileStorageService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10); // 10 minutes for large file uploads
})
.AddHttpMessageHandler<AppBlueprint.Web.Services.AuthenticationDelegatingHandler>()
.AddStandardResilienceHandler(options =>
{
    // Configure retry policy
    options.Retry.MaxRetryAttempts = 2;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    
    // Configure timeouts for file uploads - much longer than default
    options.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5); // 5 minutes per attempt
    options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10); // 10 minutes total
    
    // Configure circuit breaker with longer thresholds
    // Sampling duration must be at least double the attempt timeout (5 min * 2 = 10 min)
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

Console.WriteLine(ConsoleSeparator);
Console.WriteLine("[Web] Application built successfully");
Console.WriteLine($"[Web] Environment: {app.Environment.EnvironmentName}");
Console.WriteLine(ConsoleSeparator);

// IMPORTANT: UseForwardedHeaders must come BEFORE UseRouting
// This ensures redirect URIs use HTTPS when behind Railway's proxy
app.UseForwardedHeaders();

app.UseRouting();
// Force HTTPS for OAuth authentication to work correctly
// Disabled in development to avoid cookie issues with self-signed certificates
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    Console.WriteLine("[Web] HTTPS redirection enabled (production)");
}
else
{
    Console.WriteLine("[Web] HTTPS redirection disabled (development - avoiding cookie issues)");
}

// Serve static files FIRST - before security headers to ensure proper Content-Type is set
app.UseStaticFiles();

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
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://fonts.googleapis.com https://cdn.jsdelivr.net https://www.gstatic.com; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://www.gstatic.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' https://32nkyp.logto.app wss: ws: https://cdn.jsdelivr.net https://www.gstatic.com https://*.firebaseio.com https://*.googleapis.com;");

    // Permissions Policy - control browser features
    context.Response.Headers.Append("Permissions-Policy",
        "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

    await next();
});

app.UseAntiforgery();

// Cookie policy middleware - must come before authentication
app.UseCookiePolicy();

// Add authentication and authorization middleware for Logto
app.UseAuthentication();
app.UseAuthorization();

// ✅ Add admin IP whitelist (blocks non-whitelisted IPs from admin routes)
// Only enabled in production - disabled in development to avoid local access issues
if (!app.Environment.IsDevelopment())
{
    app.UseAdminIpWhitelist();
    Console.WriteLine("[Web] Admin IP whitelist middleware enabled (production)");
}
else
{
    Console.WriteLine("[Web] Admin IP whitelist middleware disabled (development)");
}

app.UseOutputCache();

// Map API controllers (e.g., FirebaseConfigController)
app.MapControllers();

// ⚠️ CRITICAL: Map authentication endpoints BEFORE Blazor routing
// Blazor's catch-all routing must not intercept auth callbacks
// Logto authentication endpoints - EXACTLY as per documentation
// https://docs.logto.io/quick-starts/dotnet-core/blazor-server
app.MapAuthenticationEndpoints(builder.Configuration);

// Map SignalR hubs - authentication validated in hub's OnConnectedAsync
// Note: Do NOT use .RequireAuthorization() here as it causes OIDC redirect (302)
// instead of allowing cookie auth to flow through. Hub validates auth manually.
app.MapHub<AppBlueprint.Infrastructure.SignalR.DemoChatHub>("/hubs/demochat").AllowAnonymous();
app.MapHub<AppBlueprint.Infrastructure.SignalR.NotificationHub>("/hubs/notifications").AllowAnonymous();
Console.WriteLine("[Web] SignalR hub mapped: /hubs/demochat (auth validated in hub)");
Console.WriteLine("[Web] SignalR hub mapped: /hubs/notifications (auth validated in hub)");

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly)
    .AddInteractiveServerRenderMode();

// Diagnostic endpoint to test Logto connectivity from Railway
app.MapLogtoTestEndpoint(builder.Configuration, app.Environment);

// Add callback diagnostic endpoint for debugging auth issues
if (app.Environment.IsDevelopment())
{
    app.MapGet("/callback-debug", (HttpContext context) =>
    {
        var diagnostics = new
        {
            RequestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}",
            Cookies = context.Request.Cookies.Keys.ToList(),
            QueryParameters = context.Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString()),
            IsAuthenticated = context.User?.Identity?.IsAuthenticated ?? false,
            UserName = context.User?.Identity?.Name
        };
        
        return Results.Json(diagnostics);
    }).AllowAnonymous();
    
    // Auth diagnostic endpoint to check authentication status
    app.MapGet("/auth-debug", (HttpContext context) =>
    {
        var diagnostics = new
        {
            IsAuthenticated = context.User?.Identity?.IsAuthenticated ?? false,
            AuthenticationType = context.User?.Identity?.AuthenticationType,
            UserName = context.User?.Identity?.Name,
            Claims = context.User?.Claims.Select(c => new { c.Type, c.Value }).ToList(),
            Cookies = context.Request.Cookies.Keys.ToList()
        };
        
        return Results.Json(diagnostics);
    }).AllowAnonymous();
}

app.MapDefaultEndpoints();

Console.WriteLine(ConsoleSeparator);
Console.WriteLine("[Web] Starting application...");
Console.WriteLine("[Web] Navigate to the app and watch for logs");
Console.WriteLine(ConsoleSeparator);

 await app.RunAsync();



