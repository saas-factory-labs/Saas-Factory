using Logto.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Extension methods for configuring web authentication (Logto, cookies, data protection)
/// </summary>
public static class WebAuthenticationExtensions
{
    private static readonly string[] LogtoSchemes = ["Logto.Cookie", "Logto"];
    
    /// <summary>
    /// Configures authentication services for the web application including Logto, cookies, and data protection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddWebAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Configure Data Protection for Railway deployment
        // This fixes "Unable to unprotect the message.State" errors on callback
        if (!environment.IsDevelopment())
        {
            Console.WriteLine("[Web] Configuring Data Protection for production (Railway)");
            
            // Use a persistent key storage location
            var keysPath = Path.Combine("/app", "DataProtection-Keys");
            Directory.CreateDirectory(keysPath);
            
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("AppBlueprint")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
            
            Console.WriteLine($"[Web] Data Protection keys will be stored at: {keysPath}");
        }
        else
        {
            Console.WriteLine("[Web] Using default Data Protection (development mode)");
        }

        // Check if Logto authentication is configured
        string? logtoAppId = configuration["Logto:AppId"];
        string? logtoEndpoint = configuration["Logto:Endpoint"];
        string? logtoAppSecret = configuration["Logto:AppSecret"];
        bool hasLogtoConfig = !string.IsNullOrEmpty(logtoAppId) && !string.IsNullOrEmpty(logtoEndpoint);

        if (hasLogtoConfig)
        {
            Console.WriteLine("[Web] ========================================");
            Console.WriteLine("[Web] Logto authentication configuration found");
            Console.WriteLine($"[Web] Endpoint: {logtoEndpoint}");
            Console.WriteLine($"[Web] AppId: {logtoAppId}");
            Console.WriteLine($"[Web] Has AppSecret: {!string.IsNullOrEmpty(logtoAppSecret)}");
            Console.WriteLine("[Web] ========================================");
            
            // Add Logto authentication - EXACTLY as per Logto documentation
            services.AddLogtoAuthentication(options =>
            {
                options.Endpoint = logtoEndpoint;
                options.AppId = logtoAppId;
                options.AppSecret = logtoAppSecret;
            });
            
            Console.WriteLine("[Web] Logto authentication configured successfully");
        }
        else
        {
            Console.WriteLine("[Web] ========================================");
            Console.WriteLine("[Web] Logto authentication NOT configured");
            Console.WriteLine("[Web] Running without authentication");
            Console.WriteLine("[Web] To enable authentication, set:");
            Console.WriteLine("[Web]   - Logto__AppId");
            Console.WriteLine("[Web]   - Logto__Endpoint");
            Console.WriteLine("[Web]   - Logto__AppSecret");
            Console.WriteLine("[Web] ========================================");
            
            // Add minimal authentication for API compatibility
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        // Add authorization services
        services.AddAuthorization();

        // Keep ITokenStorageService for backward compatibility with TodoService which uses it for API authentication
        // This service stores and retrieves JWT tokens from the authentication cookie
        services.AddScoped<Authorization.ITokenStorageService, 
            Authorization.TokenStorageService>();

        return services;
    }

    /// <summary>
    /// Maps authentication endpoints (sign in, sign out) for the web application.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app)
    {
        // Sign in endpoint
        // app.MapGet("/SignIn", () =>
        // {
        //     // Trigger authentication
        //     return Results.Challenge(
        //         new AuthenticationProperties { RedirectUri = "/" });
        // }).AllowAnonymous();
        
        app.MapGet("/SignIn", async context =>
        {
            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                await context.ChallengeAsync(new AuthenticationProperties { RedirectUri = "/" });
            } else {
                context.Response.Redirect("/");
            }
        });

        // Sign out endpoint  
        // app.MapGet("/SignOut", () =>
        // {
        //     // Trigger sign out using Logto SDK's registered schemes
        //     // The SDK registers: "Logto.Cookie" and "Logto" (not the default "Cookies" and "OpenIdConnect")
        //     return Results.SignOut(
        //         new AuthenticationProperties { RedirectUri = "/" },
        //         LogtoSchemes);
        // }).RequireAuthorization();
        
        app.MapGet("/SignOut", async context =>
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                await context.SignOutAsync(new AuthenticationProperties { RedirectUri = "/" });
            } else {
                context.Response.Redirect("/");
            }
        });

        return app;
    }

    /// <summary>
    /// Maps a diagnostic endpoint to test Logto connectivity (useful for Railway deployment).
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapLogtoTestEndpoint(
        this WebApplication app,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        app.MapGet("/test-logto-connection", async () =>
        {
            var results = new List<object>();
            
            // Test 1: DNS Resolution
            try
            {
                var host = "32nkyp.logto.app";
                var addresses = await System.Net.Dns.GetHostAddressesAsync(host);
                results.Add(new 
                { 
                    test = "DNS Resolution",
                    success = true,
                    host = host,
                    addresses = addresses.Select(a => a.ToString()).ToArray()
                });
            }
            catch (Exception ex)
            {
                results.Add(new 
                { 
                    test = "DNS Resolution",
                    success = false,
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
            
            // Test 2: HTTPS Connectivity (with longer timeout for Railway)
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
                var url = "https://32nkyp.logto.app/oidc/.well-known/openid-configuration";
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await client.GetAsync(url);
                sw.Stop();
                var content = await response.Content.ReadAsStringAsync();
                
                results.Add(new 
                { 
                    test = "HTTPS Connectivity",
                    success = response.IsSuccessStatusCode,
                    url = url,
                    statusCode = (int)response.StatusCode,
                    statusDescription = response.ReasonPhrase,
                    contentLength = content.Length,
                    elapsedMs = sw.ElapsedMilliseconds,
                    warning = sw.ElapsedMilliseconds > 5000 ? "⚠️ Slow connection - took over 5 seconds" : null
                });
            }
            catch (Exception ex)
            {
                results.Add(new 
                { 
                    test = "HTTPS Connectivity",
                    success = false,
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
            
            // Test 3: Current Logto Configuration
            var logtoConfig = new
            {
                test = "Logto Configuration",
                endpoint = configuration["Logto:Endpoint"],
                appId = configuration["Logto:AppId"],
                hasAppSecret = !string.IsNullOrEmpty(configuration["Logto:AppSecret"]),
                environment = environment.EnvironmentName,
                requiredRedirectUris = new[]
                {
                    "https://appblueprint-web-staging.up.railway.app/callback",
                    "https://appblueprint-web-staging.up.railway.app/signout-callback-logto"
                },
                requiredPostLogoutRedirectUris = new[]
                {
                    "https://appblueprint-web-staging.up.railway.app/"
                },
                configurationInstructions = "Add these URIs to your Logto application configuration under 'Redirect URIs' and 'Post sign-out redirect URIs'"
            };
            results.Add(logtoConfig);
            
            return Results.Json(new 
            { 
                timestamp = DateTime.UtcNow,
                railway = !environment.IsDevelopment(),
                tests = results,
                networkIssue = "⚠️ CRITICAL: Railway cannot reach Logto (30s+ timeout). This is a Railway network/firewall issue.",
                action = "Authentication will likely fail until Railway can establish connection to Logto servers.",
                workaround = "Increased backchannel timeout to 60s - authentication may work but will be very slow",
                redirectUrisConfigured = "Make sure these URIs are in Logto: https://appblueprint-web-staging.up.railway.app/callback"
            }, 
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        });

        return app;
    }
}

