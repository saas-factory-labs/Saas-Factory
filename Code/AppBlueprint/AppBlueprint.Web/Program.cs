using AppBlueprint.Api.Client.Sdk;
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
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

builder.WebHost.ConfigureKestrel(options =>
{
    // Always listen on HTTP port 80
    options.ListenAnyIP(80);
    
    // Only configure HTTPS in Development mode
    // In production (Railway), TLS is handled at the edge/load balancer
    if (builder.Environment.IsDevelopment())
    {
        string certPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ASP.NET", "Https", "web-service.pfx");
        string certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD") ?? "";

        options.ListenAnyIP(443, listenOptions =>
        {
            if (File.Exists(certPath))
            {
                try
                {
                    var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword, X509KeyStorageFlags.DefaultKeySet);
                    listenOptions.UseHttps(cert);
                    Console.WriteLine("[Web] HTTPS configured with custom certificate");
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    Console.WriteLine($"Certificate error: {ex.Message}. Using default HTTPS.");
                    listenOptions.UseHttps();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Certificate access error: {ex.Message}. Using default HTTPS.");
                    listenOptions.UseHttps();
                }
            }
            else
            {
                listenOptions.UseHttps();
                Console.WriteLine("[Web] HTTPS configured with default dev certificate");
            }
        });
    }
    else
    {
        Console.WriteLine("[Web] Production mode - HTTPS disabled (handled by Railway edge)");
    }
});

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
builder.Services.AddMudServices();
builder.Services.AddSingleton<BreadcrumbService>();
builder.Services.AddUiKit();

// Add HttpContextAccessor for accessing authentication tokens in delegating handlers
builder.Services.AddHttpContextAccessor();

// Add OpenID Connect authentication (standard OIDC instead of Logto-specific SDK)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://32nkyp.logto.app/oidc";
    options.ClientId = builder.Configuration["Logto:AppId"]!;
    options.ClientSecret = builder.Configuration["Logto:AppSecret"];
    options.ResponseType = "code";
    options.ResponseMode = "query";  // Use query instead of form_post
    options.UsePkce = false;  // Disable PKCE - Logto may not support it properly
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;  // Allow HTTP in development
    
    // Add required scopes
    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    
    // Configure callback paths
    options.CallbackPath = "/callback";
    options.SignedOutCallbackPath = "/signout-callback-logto";
    
    // Map claims
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role",
        ValidateIssuer = true
    };
    
    // Add event handlers for debugging
    options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            Console.WriteLine($"[OIDC] Redirecting to identity provider: {context.ProtocolMessage.IssuerAddress}");
            Console.WriteLine($"[OIDC] Redirect URI: {context.ProtocolMessage.RedirectUri}");
            return Task.CompletedTask;
        },
        OnAuthorizationCodeReceived = context =>
        {
            Console.WriteLine($"[OIDC] Authorization code received");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"[OIDC] Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[OIDC] Authentication failed: {context.Exception?.Message}");
            return Task.CompletedTask;
        }
    };
    
    Console.WriteLine($"[Web] OpenID Connect configured with Authority: {options.Authority}");
});

// Configure cookie authentication to work with Blazor Server
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/signin-logto";
    options.LogoutPath = "/signout-logto";
    options.AccessDeniedPath = "/access-denied";
});

// Add authorization services
builder.Services.AddAuthorization();

// Register stub IUserAuthenticationProvider for backward compatibility with UiKit components
// UiKit components (NavigationMenu, Appbar, etc.) still depend on this interface
// This stub integrates with ASP.NET Core's authentication state
builder.Services.AddScoped<AppBlueprint.Infrastructure.Authorization.IUserAuthenticationProvider, 
    AppBlueprint.Infrastructure.Authorization.AspNetCoreAuthenticationProviderStub>();

// Register IAuthenticationProvider (Kiota) using the same stub
builder.Services.AddScoped<IAuthenticationProvider>(sp => 
    sp.GetRequiredService<AppBlueprint.Infrastructure.Authorization.IUserAuthenticationProvider>());

// Keep ITokenStorageService for backward compatibility (used by TodoService)
builder.Services.AddScoped<AppBlueprint.Infrastructure.Authorization.ITokenStorageService, 
    AppBlueprint.Infrastructure.Authorization.TokenStorageService>();

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



var app = builder.Build();

Console.WriteLine("========================================");
Console.WriteLine("[Web] Application built successfully");
Console.WriteLine($"[Web] Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("========================================");

app.UseRouting();
// app.UseHttpsRedirection(); // Temporarily disabled for design review
app.UseStaticFiles();
app.UseAntiforgery();

// Add authentication and authorization middleware for Logto
app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly)
    .AddInteractiveServerRenderMode();

// Map OpenID Connect authentication endpoints
app.MapGet("/signin-logto", async (HttpContext context) =>
{
    // Check if user is already authenticated
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine($"[Web] /signin-logto - User already authenticated: {context.User.Identity.Name}");
        
        // Get return URL or default to "/"
        var returnUrl = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";
        context.Response.Redirect(returnUrl);
        return;
    }
    
    Console.WriteLine("[Web] /signin-logto endpoint hit - triggering OpenID Connect challenge");
    
    // Get the return URL from query string, or default to "/"
    var returnUrl2 = context.Request.Query["returnUrl"].FirstOrDefault() ?? "/";
    
    // Trigger authentication challenge with OpenID Connect
    await context.ChallengeAsync(
        Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme,
        new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            RedirectUri = returnUrl2
        });
}).AllowAnonymous();

app.MapGet("/signout-logto", async (HttpContext context) =>
{
    Console.WriteLine("[Web] /signout-logto endpoint hit - signing out");
    
    // Sign out from both OpenID Connect and cookie authentication
    await context.SignOutAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme);
    await context.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
    
    // Redirect to home page
    context.Response.Redirect("/");
}).RequireAuthorization();

app.MapDefaultEndpoints();

Console.WriteLine("========================================");
Console.WriteLine("[Web] Starting application...");
Console.WriteLine("[Web] Navigate to the app and watch for logs");
Console.WriteLine("========================================");

app.Run();


// using AppBlueprint.Api.Client.Sdk;
// using AppBlueprint.Infrastructure.Authorization;
// using AppBlueprint.UiKit;
// using AppBlueprint.UiKit.Models;
// using AppBlueprint.Web;
// using AppBlueprint.Web.Components;
// using Microsoft.Kiota.Abstractions;
// using Microsoft.Kiota.Abstractions.Authentication;
// using Microsoft.Kiota.Http.HttpClientLibrary;
// using MudBlazor.Services;
// using _Imports = AppBlueprint.UiKit._Imports;
// using System.Security.Cryptography.X509Certificates;
// using System.IO;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Diagnostics.HealthChecks;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;

// var builder = WebApplication.CreateBuilder(args);
// builder.Logging.ClearProviders();
// builder.Logging.AddConsole();
// builder.AddServiceDefaults();

// builder.Host.UseDefaultServiceProvider((context, options) =>
// {
//     options.ValidateScopes = true;
//     options.ValidateOnBuild = true;
// });

// var navigationRoutes = builder.Configuration
//     .GetSection("Navigation:Routes")
//     .Get<List<NavLinkMetadata>>() ?? new List<NavLinkMetadata>();
// builder.Services.AddSingleton(navigationRoutes);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(80);
//     string certPath = Path.Combine(
//         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
//         "ASP.NET", "Https", "web-service.pfx");
//     string certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
//     if (string.IsNullOrEmpty(certPassword)) certPassword = "dev-cert-password";
//     if (File.Exists(certPath))
//     {
//         options.ListenAnyIP(443, listenOptions =>
//         {
//             listenOptions.UseHttps(httpsOptions =>
//             {
//                 var certs = new X509Certificate2Collection();
//                 certs.Import(certPath, certPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
//                 httpsOptions.ServerCertificate = certs[0];
//             });
//         });
//     }
//     else
//     {
//         options.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
//     }
// });

// builder.Services.ConfigureHttpClientDefaults(http =>
// {
//     if (builder.Environment.IsDevelopment())
//     {
//         http.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
//         {
//             ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
//         });
//     }
// });

// builder.Services.AddOutputCache();
// builder.Services.AddRazorComponents().AddInteractiveServerComponents();
// builder.Services.AddMudServices();
// builder.Services.AddSingleton<BreadcrumbService>();
// builder.Services.AddUiKit();

// // Add authentication services using the factory pattern
// builder.Services.AddAuthenticationServices();

// builder.Services.AddScoped<IRequestAdapter>(sp => new HttpClientRequestAdapter(sp.GetRequiredService<IAuthenticationProvider>())
// {
//     BaseUrl = "https://localhost:5002"
// });
// builder.Services.AddScoped<ApiClient>(sp => new ApiClient(sp.GetRequiredService<IRequestAdapter>()));

// var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     app.UseHsts();
// }

// app.UseRouting();
// app.UseHttpsRedirection();
// app.UseStaticFiles();
// app.UseAntiforgery();
// app.UseOutputCache();

// app.MapRazorComponents<App>()
//     .AddAdditionalAssemblies(typeof(_Imports).Assembly)
//     .AddInteractiveServerRenderMode();

// app.MapDefaultEndpoints();
// app.Run();

// // using AppBlueprint.Api.Client.Sdk;
// // using AppBlueprint.Infrastructure.Authorization;
// // using AppBlueprint.UiKit;
// // using AppBlueprint.UiKit.Models;
// // using AppBlueprint.Web;
// // using AppBlueprint.Web.Components;
// // using Microsoft.Kiota.Abstractions;
// // using Microsoft.Kiota.Abstractions.Authentication;
// // using Microsoft.Kiota.Http.HttpClientLibrary;
// // using MudBlazor.Services;
// // using _Imports = AppBlueprint.UiKit._Imports;
// // using System.Security.Cryptography.X509Certificates;
// // using System.IO;
// // using Microsoft.AspNetCore.Builder;
// // using Microsoft.Extensions.DependencyInjection;
// // using Microsoft.Extensions.Hosting;
// // using Microsoft.Extensions.Logging;

// // var builder = WebApplication.CreateBuilder(args);

// // // Add service defaults & Aspire components BEFORE other services.
// // builder.AddServiceDefaults();

// // // Log OTLP settings for verification
// // Console.WriteLine($"[WebFrontend] OTLP endpoint → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")}\n" +
// //                   $"[WebFrontend] OTLP protocol → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")}\n");

// // builder.Host.UseDefaultServiceProvider((context, options) =>
// // {
// //     options.ValidateScopes = true;
// //     options.ValidateOnBuild = true;
// // });

// // // Read navigation routes from configuration.
// // var navigationRoutes = builder.Configuration
// //     .GetSection("Navigation:Routes")
// //     .Get<List<NavLinkMetadata>>() ?? new List<NavLinkMetadata>();
// // builder.Services.AddSingleton(navigationRoutes);

// // // Configure Kestrel to listen on both HTTP and HTTPS
// // builder.WebHost.ConfigureKestrel(serverOptions => 
// // {
// //     serverOptions.ListenAnyIP(80);
// //     string certPath = Path.Combine(
// //         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
// //         "ASP.NET", "Https", "web-service.pfx");

// //     string certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
// //     if (string.IsNullOrEmpty(certPassword))
// //     {
// //         certPassword = "dev-cert-password";
// //     }

// //     try
// //     {
// //         if (File.Exists(certPath))
// //         {
// //             serverOptions.ListenAnyIP(443, listenOptions =>
// //             {
// //                 listenOptions.UseHttps(options =>
// //                 {
// //                     var certCollection = new X509Certificate2Collection();
// //                     certCollection.Import(certPath, certPassword, X509KeyStorageFlags.Exportable |
// //                         X509KeyStorageFlags.PersistKeySet |
// //                         X509KeyStorageFlags.MachineKeySet);
// //                     options.ServerCertificate = certCollection[0];
// //                     Console.WriteLine($"Using certificate from: {certPath}");
// //                 });
// //             });
// //         }
// //         else
// //         {
// //             Console.WriteLine($"Certificate not found at {certPath}. Using default development certificate.");
// //             serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
// //         }
// //     }
// //     catch (Exception ex)
// //     {
// //         Console.WriteLine($"Error configuring HTTPS: {ex.Message}");
// //         serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
// //     }
// // });

// // // Configure HTTP clients and logging
// // builder.Services.ConfigureHttpClientDefaults(httpClientBuilder =>
// // {
// //     if (builder.Environment.IsDevelopment())
// //     {
// //         httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() =>
// //             new HttpClientHandler
// //             {
// //                 ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
// //             });
// //     }
// // });

// // builder.Logging.ClearProviders();
// // builder.Logging.AddConsole();

// // builder.Services.AddOutputCache();

// // // Add UI services
// // builder.Services.AddRazorComponents()
// //     .AddInteractiveServerComponents();
// // builder.Services.AddMudServices();
// // builder.Services.AddSingleton<BreadcrumbService>();
// // builder.Services.AddUiKit();

// // // Authentication & API client
// // const string authEndpoint = "https://nnihwwacvgqfbkxzjnvx.supabase.co/auth/v1/token";
// // builder.Services.AddHttpClient("authClient");
// // builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
// // builder.Services.AddScoped<UserAuthenticationProvider>(sp =>
// // {
// //     var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
// //     var httpClient = clientFactory.CreateClient("authClient");
// //     var tokenStorage = sp.GetRequiredService<ITokenStorageService>();
// //     return new UserAuthenticationProvider(httpClient, authEndpoint, tokenStorage);
// // });
// // builder.Services.AddScoped<IUserAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
// // builder.Services.AddScoped<IAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
// // builder.Services.AddScoped<IRequestAdapter>(sp => new HttpClientRequestAdapter(sp.GetRequiredService<IAuthenticationProvider>())
// // {
// //     BaseUrl = "http://localhost:8090"
// // });
// // builder.Services.AddScoped<ApiClient>(sp => new ApiClient(sp.GetRequiredService<IRequestAdapter>()));

// // var app = builder.Build();

// // if (!app.Environment.IsDevelopment())
// // {
// //     app.UseExceptionHandler("/Error", true);
// //     app.UseHsts();
// // }

// // app.UseHttpsRedirection();
// // app.UseStaticFiles();
// // app.UseAntiforgery();
// // app.UseOutputCache();

// // app.MapRazorComponents<App>()
// //     .AddAdditionalAssemblies(typeof(_Imports).Assembly)
// //     .AddInteractiveServerRenderMode();

// // app.MapDefaultEndpoints();
// // app.MapGet("/health", () => Results.Ok("Healthy"));

// // app.Run();


// // // using AppBlueprint.Api.Client.Sdk;
// // // using AppBlueprint.Infrastructure.Authorization;
// // // using AppBlueprint.UiKit;
// // // using AppBlueprint.UiKit.Models;
// // // using AppBlueprint.Web;
// // // using AppBlueprint.Web.Components;
// // // using Microsoft.Kiota.Abstractions;
// // // using Microsoft.Kiota.Abstractions.Authentication;
// // // using Microsoft.Kiota.Http.HttpClientLibrary;
// // // using MudBlazor.Services;
// // // using _Imports = AppBlueprint.UiKit._Imports;
// // // using System.Security.Cryptography.X509Certificates;
// // // using System.IO;

// // // WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// // // // Add service defaults & Aspire components BEFORE other services.
// // // builder.AddServiceDefaults();

// // // builder.Host.UseDefaultServiceProvider((context, options) =>
// // // {
// // //     options.ValidateScopes = true;
// // //     options.ValidateOnBuild = true;
// // // });

// // // // Read navigation routes from configuration.
// // // List<NavLinkMetadata> navigationRoutes = builder.Configuration
// // //     .GetSection("Navigation:Routes")
// // //     .Get<List<NavLinkMetadata>>() ?? new List<NavLinkMetadata>();
// // // builder.Services.AddSingleton(navigationRoutes);

// // // // Configure Kestrel to listen on both HTTP and HTTPS
// // // builder.WebHost.ConfigureKestrel(serverOptions => 
// // // { 
// // //     // Listen on HTTP port 80
// // //     serverOptions.ListenAnyIP(80); 

// // //     // Listen on HTTPS port 443 with certificate from ASP.NET Https folder
// // //     string certPath = Path.Combine(
// // //         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
// // //         "ASP.NET", "Https", "web-service.pfx");

// // //     // Get certificate password from environment variable (set by AppHost) or use default
// // //     string certPassword = Environment.GetEnvironmentVariable("CERTIFICATE_PASSWORD");
// // //     if (string.IsNullOrEmpty(certPassword))
// // //     {
// // //         certPassword = "dev-cert-password";
// // //     }

// // //     try
// // //     {
// // //         if (File.Exists(certPath))
// // //         {
// // //             serverOptions.ListenAnyIP(443, listenOptions =>
// // //             {
// // //                 listenOptions.UseHttps(options =>
// // //                 {
// // //                     try
// // //                     {
// // //                         // Use X509Certificate2Collection to load the certificate correctly
// // //                         var certCollection = new X509Certificate2Collection();
// // //                         certCollection.Import(certPath, certPassword, X509KeyStorageFlags.Exportable | 
// // //                                                                     X509KeyStorageFlags.PersistKeySet | 
// // //                                                                     X509KeyStorageFlags.MachineKeySet);

// // //                         // Get the certificate from the collection
// // //                         options.ServerCertificate = certCollection[0];
// // //                         Console.WriteLine($"Using certificate from: {certPath}");
// // //                     }
// // //                     catch (Exception ex)
// // //                     {
// // //                         Console.WriteLine($"Error loading certificate from {certPath}: {ex.Message}");

// // //                         // Fallback to development certificate
// // //                         Console.WriteLine("Falling back to the default development certificate");
// // //                         // Let ASP.NET Core use its default developer certificate
// // //                     }
// // //                 });
// // //             });
// // //         }
// // //         else
// // //         {
// // //             // If our specific certificate doesn't exist, we need to add a development certificate
// // //             Console.WriteLine($"Certificate not found at {certPath}. Using default development certificate.");
// // //             serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
// // //         }
// // //     }
// // //     catch (Exception ex)
// // //     {
// // //         Console.WriteLine($"Error configuring HTTPS: {ex.Message}");
// // //         serverOptions.ListenAnyIP(443, listenOptions => listenOptions.UseHttps());
// // //     }
// // // });

// // // // Configure service to ignore certificate validation errors in development
// // // // This is useful when the browser doesn't trust our development certificate
// // // builder.Services.ConfigureHttpClientDefaults(httpClientBuilder =>
// // // {
// // //     if (builder.Environment.IsDevelopment())
// // //     {
// // //         httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => 
// // //             new HttpClientHandler
// // //             {
// // //                 ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
// // //             });
// // //     }
// // // });

// // // // Configure logging.
// // // builder.Logging.ClearProviders();
// // // builder.Logging.AddConsole();

// // // builder.Services.AddOutputCache();

// // // // Add services to the container.
// // // builder.Services.AddRazorComponents()
// // //     .AddInteractiveServerComponents();

// // // builder.Services.AddMudServices();
// // // builder.Services.AddSingleton<BreadcrumbService>();
// // // builder.Services.AddUiKit();

// // // // Set the auth endpoint. (This endpoint should return a JSON with AccessToken and ExpiresIn.)
// // // const string authEndpoint = "https://nnihwwacvgqfbkxzjnvx.supabase.co/auth/v1/token";

// // // // Register a named HttpClient for authentication.
// // // builder.Services.AddHttpClient("authClient");

// // // // Register the token storage service
// // // builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();

// // // // Register the UserAuthenticationProvider with the token storage service
// // // // Changed from AddSingleton to AddScoped to match ITokenStorageService lifetime
// // // builder.Services.AddScoped<UserAuthenticationProvider>(sp =>
// // // {
// // //     IHttpClientFactory clientFactory = sp.GetRequiredService<IHttpClientFactory>();
// // //     HttpClient httpClient = clientFactory.CreateClient("authClient");
// // //     ITokenStorageService tokenStorage = sp.GetRequiredService<ITokenStorageService>();
// // //     return new UserAuthenticationProvider(httpClient, authEndpoint, tokenStorage);
// // // });

// // // // Expose the UserAuthenticationProvider as the IUserAuthenticationProvider
// // // // Changed from AddSingleton to AddScoped to match UserAuthenticationProvider lifetime
// // // builder.Services.AddScoped<IUserAuthenticationProvider>(sp =>
// // //     sp.GetRequiredService<UserAuthenticationProvider>());

// // // // Expose the UserAuthenticationProvider as the IAuthenticationProvider
// // // // Changed from AddSingleton to AddScoped to match UserAuthenticationProvider lifetime
// // // builder.Services.AddScoped<IAuthenticationProvider>(sp =>
// // //     sp.GetRequiredService<UserAuthenticationProvider>());

// // // // Register Kiota Request Adapter.
// // // builder.Services.AddScoped<IRequestAdapter>(sp =>
// // // {
// // //     IAuthenticationProvider authProvider = sp.GetRequiredService<IAuthenticationProvider>();
// // //     return new HttpClientRequestAdapter(authProvider)
// // //     {
// // //         // https://AppBlueprint-Api
// // //         BaseUrl = "http://localhost:8090" // Adjust this to your API's base URL.
// // //     };
// // // });

// // // // Register Kiota API Client.
// // // builder.Services.AddScoped<ApiClient>(sp =>
// // // {
// // //     IRequestAdapter adapter = sp.GetRequiredService<IRequestAdapter>();
// // //     return new ApiClient(adapter);
// // // });

// // // WebApplication app = builder.Build();

// // // if (!app.Environment.IsDevelopment())
// // // {
// // //     app.UseExceptionHandler("/Error", true);
// // //     app.UseHsts();
// // // }

// // // app.UseHttpsRedirection(); // Enable HTTPS redirection
// // // app.UseStaticFiles();
// // // app.UseAntiforgery();
// // // app.UseOutputCache();

// // // app.MapRazorComponents<App>()
// // //     .AddAdditionalAssemblies(typeof(_Imports).Assembly)
// // //     .AddInteractiveServerRenderMode();

// // // app.MapDefaultEndpoints();

// // // // Health check endpoint.
// // // app.MapGet("/health", () => Results.Ok("Healthy"));

// // // app.Run();

