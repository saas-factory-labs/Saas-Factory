using Logto.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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
            
            services.Configure<OpenIdConnectOptions>(LogtoDefaults.AuthenticationScheme, options =>
            {
                options.SaveTokens = true;
                options.Scope.Add("offline_access");
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
    /// <param name="configuration">The configuration to get Logto endpoint</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app, IConfiguration configuration)
    {
        // Sign in endpoint
        app.MapGet("/SignIn", async context =>
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[Web] SignIn endpoint called");
            Console.WriteLine($"[Web] User authenticated: {context.User?.Identity?.IsAuthenticated ?? false}");
            Console.WriteLine($"[Web] User name: {context.User?.Identity?.Name ?? "null"}");
            Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            
            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                Console.WriteLine("[Web] User NOT authenticated - calling ChallengeAsync to redirect to Logto");
                Console.WriteLine("[Web] Redirect URI after login: /");
                
                try
                {
                    await context.ChallengeAsync(new AuthenticationProperties { RedirectUri = "/" });
                    Console.WriteLine("[Web] ✅ ChallengeAsync completed - should redirect to Logto now");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Web] ❌ ERROR in ChallengeAsync: {ex.Message}");
                    Console.WriteLine($"[Web] Stack trace: {ex.StackTrace}");
                }
            }
            else
            {
                Console.WriteLine("[Web] User already authenticated - redirecting to /");
                context.Response.Redirect("/");
            }
            
            Console.WriteLine("========================================");
        });

        // Sign out endpoint - Full Logto sign-out (signs out from both app and Logto IdP)
        // Sign out endpoint - Full Logto sign-out
        app.MapGet("/SignOut", async (HttpContext context, IConfiguration config) =>
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[Web] SignOut endpoint called - FULL LOGTO SIGN-OUT");
            Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            Console.WriteLine($"[Web] User authenticated: {context.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"[Web] User name: {context.User?.Identity?.Name}");
            
            // Get the id_token from the authentication properties if available
            var authenticateResult = await context.AuthenticateAsync("Logto");
            string? idToken = authenticateResult?.Properties?.GetTokenValue("id_token");
            
            Console.WriteLine($"[Web] ID Token available: {idToken != null}");
            
            // Clear local authentication cookies first
            await context.SignOutAsync("Logto.Cookie");
            Console.WriteLine("[Web] ✅ Cleared Logto.Cookie");
            
            // Build Logto's end session URL
            var logtoEndpoint = config["Logto:Endpoint"];
            
            // Construct the post-logout redirect URI
            // The web runs on 8083 (HTTPS) and 8082 (HTTP) in development, port 80 in production
            var request = context.Request;
            var postLogoutRedirectUri = $"{request.Scheme}://{request.Host}/logout-complete";
            
            Console.WriteLine($"[Web] Logto endpoint: {logtoEndpoint}");
            Console.WriteLine($"[Web] Request Host: {request.Host}");
            Console.WriteLine($"[Web] Post-logout redirect URI: {postLogoutRedirectUri}");
            
            // Construct the Logto end session URL according to OIDC spec
            var endSessionUrl = $"{logtoEndpoint}oidc/session/end?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri)}";
            
            // Add id_token_hint if available (recommended for proper sign-out)
            if (!string.IsNullOrEmpty(idToken))
            {
                endSessionUrl += $"&id_token_hint={Uri.EscapeDataString(idToken)}";
                Console.WriteLine("[Web] ✅ Added id_token_hint to sign-out URL");
            }
            else
            {
                Console.WriteLine("[Web] ⚠️ No id_token available - sign-out might not work properly");
            }
            
            Console.WriteLine($"[Web] ➡️  Redirecting to Logto end session:");
            Console.WriteLine($"[Web] {endSessionUrl}");
            Console.WriteLine("========================================");
            
            // Redirect to Logto's end session endpoint
            context.Response.Redirect(endSessionUrl);
        }).AllowAnonymous();

        // Logout complete callback - where Logto redirects back after sign-out
        app.MapGet("/logout-complete", async (HttpContext context) =>
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[Web] Logout complete callback - user returned from Logto");
            Console.WriteLine($"[Web] Current authentication state: {context.User?.Identity?.IsAuthenticated}");
            
            // Make absolutely sure cookies are cleared
            try
            {
                await context.SignOutAsync("Logto.Cookie");
                await context.SignOutAsync("Logto");
                Console.WriteLine("[Web] ✅ Cleared all authentication cookies");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Web] Cookie clear error (might be already cleared): {ex.Message}");
            }
            
            // Return HTML that uses JavaScript to navigate to login
            // This ensures a complete page reload and new Blazor circuit
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
<head>
    <title>Signing out...</title>
    <script>
        console.log('[Logout] Redirecting to login page after sign-out');
        window.location.href = '/login';
    </script>
</head>
<body>
    <p>Signing out... You will be redirected to the login page.</p>
    <p>If not redirected automatically, <a href='/login'>click here</a>.</p>
</body>
</html>
");
            Console.WriteLine("[Web] ✅ Sent HTML redirect to /login");
            Console.WriteLine("========================================");
        }).AllowAnonymous();

        // Simple manual sign-out endpoint for debugging (bypasses Logto's end session endpoint)
        // This just clears local cookies without going to Logto
        app.MapGet("/SignOut/Local", async (HttpContext context) =>
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[Web] Local sign-out endpoint called (bypassing Logto end session)");
            Console.WriteLine($"[Web] User was: {context.User?.Identity?.Name ?? "unknown"}");
            
            try
            {
                // Clear local authentication cookies
                await context.SignOutAsync("Logto.Cookie");
                Console.WriteLine("[Web] ✅ Cleared Logto.Cookie");
                
                // Return HTML that forces a complete page reload to /login
                // This ensures Blazor circuit is completely reset
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<!DOCTYPE html>
<html>
<head>
    <title>Signing out...</title>
    <meta http-equiv='refresh' content='0; url=/login'>
    <script>
        console.log('[SignOut/Local] Redirecting to login page');
        window.location.replace('/login');
    </script>
</head>
<body>
    <p>Signing out... You will be redirected to the login page.</p>
    <p>If not redirected automatically, <a href='/login'>click here</a>.</p>
</body>
</html>
");
                Console.WriteLine("[Web] ✅ Sent HTML redirect to /login (forced reload)");
                Console.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Web] ⚠️ ERROR during local sign-out: {ex.Message}");
                Console.WriteLine("========================================");
                context.Response.Redirect("/login");
            }
        }).AllowAnonymous();

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

