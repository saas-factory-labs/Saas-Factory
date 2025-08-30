using AppBlueprint.Api.Client.Sdk;
using AppBlueprint.Infrastructure.Authorization;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure telemetry - must come before AddServiceDefaults
// Get the OTLP endpoint from environment or use Aspire default
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
#pragma warning disable CA1303 // Do not pass literals as localized parameters - Diagnostic logging not intended for localization
    Console.WriteLine($"[Web] Using default OTLP endpoint: {otlpDefaultEndpoint}");
#pragma warning restore CA1303
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

// Print final configuration
Console.WriteLine($"[Web] Final OTLP endpoint → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")}");
Console.WriteLine($"[Web] Final OTLP protocol → {Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL")}");

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
    options.ListenAnyIP(80);
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
        }
    });
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

const string authEndpoint = "https://nnihwwacvgqfbkxzjnvx.supabase.co/auth/v1/token";
builder.Services.AddHttpClient("authClient");
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<UserAuthenticationProvider>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("authClient");
    var storage = sp.GetRequiredService<ITokenStorageService>();
    return new UserAuthenticationProvider(client, authEndpoint, storage);
});
builder.Services.AddScoped<IUserAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
builder.Services.AddScoped<IAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
builder.Services.AddScoped<IRequestAdapter>(sp =>
    new HttpClientRequestAdapter(sp.GetRequiredService<IAuthenticationProvider>())
    {
        BaseUrl = "https://localhost:5002"
    });
builder.Services.AddScoped<ApiClient>(sp => new ApiClient(sp.GetRequiredService<IRequestAdapter>()));

var app = builder.Build();

app.UseRouting();
// app.UseHttpsRedirection(); // Temporarily disabled for design review
app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(_Imports).Assembly)
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
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

// const string authEndpoint = "https://nnihwwacvgqfbkxzjnvx.supabase.co/auth/v1/token";
// builder.Services.AddHttpClient("authClient");
// builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
// builder.Services.AddScoped<UserAuthenticationProvider>(sp =>
// {
//     var factory = sp.GetRequiredService<IHttpClientFactory>();
//     var client = factory.CreateClient("authClient");
//     var storage = sp.GetRequiredService<ITokenStorageService>();
//     return new UserAuthenticationProvider(client, authEndpoint, storage);
// });
// builder.Services.AddScoped<IUserAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
// builder.Services.AddScoped<IAuthenticationProvider>(sp => sp.GetRequiredService<UserAuthenticationProvider>());
// builder.Services.AddScoped<IRequestAdapter>(sp => new HttpClientRequestAdapter(sp.GetRequiredService<IAuthenticationProvider>())
// {
//     BaseUrl = "https://appblueprint-api:5002"
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
// //                     certCollection.Import(certPath, certPassword,
// //                         X509KeyStorageFlags.Exportable |
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
// // builder.Services.AddScoped<IRequestAdapter>(sp =>
// // {
// //     var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
// //     return new HttpClientRequestAdapter(authProvider)
// //     {
// //         BaseUrl = "http://localhost:8090"
// //     };
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
// // //             serverOptions.ListenAnyIP(443, listenOptions => 
// // //             {
// // //                 listenOptions.UseHttps(); // Uses default ASP.NET Core development certificate
// // //             });
// // //         }
// // //     }
// // //     catch (Exception ex)
// // //     {
// // //         Console.WriteLine($"Error configuring HTTPS: {ex.Message}");
// // //         // Ensure we still have HTTPS endpoint even if configuration fails
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

