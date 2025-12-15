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
using AppBlueprint.Infrastructure.Configuration;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Extension methods for configuring web authentication (Logto, cookies, data protection)
/// </summary>
public static class WebAuthenticationExtensions
{
    // Constants for authentication schemes
    private const string LogtoCookieScheme = "Logto.Cookie";
    private const string LogtoScheme = "Logto";
    
    // Constants for configuration keys
    private const string LogtoEndpointKey = "Logto:Endpoint";
    private const string LogtoAppIdKey = "Logto:AppId";
    private const string LogtoAppSecretKey = "Logto:AppSecret";
    private const string LogtoResourceKey = "Logto:Resource";
    
    // Constants for scopes and claims
    private const string ReadTodosScope = "read:todos";
    
    // Constants for paths and URIs
    private const string DataProtectionKeysPath = "/app";
    private const string DataProtectionKeysFolderName = "DataProtection-Keys";
    private const string ApplicationName = "AppBlueprint";
    
    // Constants for logging
    private const string LogSeparator = "========================================";
    
    // HTML templates
    private const string LogoutCompleteHtml = @"
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
</html>";

    private const string LocalSignOutHtml = @"
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
</html>";
    
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
        ArgumentNullException.ThrowIfNull(configuration);

        ConfigureDataProtection(services, environment);

        bool hasLogtoConfig = ConfigurationValidator.ValidateLogtoConfiguration(configuration, throwOnMissing: false);

        if (hasLogtoConfig)
        {
            ConfigureLogtoAuthentication(services, configuration, environment);
        }
        else
        {
            ConfigureFallbackAuthentication(services);
        }

        services.AddAuthorization();
        services.AddScoped<Authorization.ITokenStorageService, Authorization.TokenStorageService>();

        return services;
    }

    /// <summary>
    /// Configures Data Protection for production deployments.
    /// </summary>
    private static void ConfigureDataProtection(IServiceCollection services, IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            Console.WriteLine("[Web] Configuring Data Protection for production (Railway)");
            
            var keysPath = Path.Combine(DataProtectionKeysPath, DataProtectionKeysFolderName);
            Directory.CreateDirectory(keysPath);
            
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName(ApplicationName)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
            
            Console.WriteLine($"[Web] Data Protection keys will be stored at: {keysPath}");
        }
        else
        {
            Console.WriteLine("[Web] Using default Data Protection (development mode)");
        }
    }

    /// <summary>
    /// Configures Logto authentication with OpenID Connect.
    /// </summary>
    private static void ConfigureLogtoAuthentication(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        string? logtoAppId = configuration[LogtoAppIdKey];
        string? logtoEndpoint = configuration[LogtoEndpointKey];
        string? logtoAppSecret = configuration[LogtoAppSecretKey];
        string? logtoResource = configuration[LogtoResourceKey];
        
        LogLogtoConfiguration(logtoEndpoint, logtoAppId, logtoAppSecret, logtoResource);
        
        services.AddLogtoAuthentication(options =>
        {
            options.Endpoint = logtoEndpoint ?? string.Empty;
            options.AppId = logtoAppId ?? string.Empty;
            options.AppSecret = logtoAppSecret;
            
            ConfigureLogtoResource(options, logtoResource);
        });
        
        ConfigureOpenIdConnectOptions(services, environment);
        
        Console.WriteLine("[Web] Logto authentication configured successfully");
    }

    /// <summary>
    /// Configures fallback authentication when Logto is not available.
    /// </summary>
    private static void ConfigureFallbackAuthentication(IServiceCollection services)
    {
        Console.WriteLine($"[Web] {LogSeparator}");
        Console.WriteLine("[Web] Logto authentication NOT configured");
        Console.WriteLine("[Web] Running without authentication");
        Console.WriteLine("[Web] To enable authentication, set:");
        Console.WriteLine("[Web]   - Logto__AppId");
        Console.WriteLine("[Web]   - Logto__Endpoint");
        Console.WriteLine("[Web]   - Logto__AppSecret");
        Console.WriteLine($"[Web] {LogSeparator}");
        
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Logs Logto configuration details.
    /// </summary>
    private static void LogLogtoConfiguration(
        string? endpoint,
        string? appId,
        string? appSecret,
        string? resource)
    {
        Console.WriteLine($"[Web] {LogSeparator}");
        Console.WriteLine("[Web] Logto authentication configuration found");
        Console.WriteLine($"[Web] Endpoint: {endpoint}");
        Console.WriteLine($"[Web] AppId: {appId}");
        Console.WriteLine($"[Web] Has AppSecret: {!string.IsNullOrEmpty(appSecret)}");
        Console.WriteLine($"[Web] API Resource: {resource ?? "(not set - will receive opaque tokens)"}");
        Console.WriteLine($"[Web] {LogSeparator}");
    }

    /// <summary>
    /// Configures Logto API resource and scopes.
    /// </summary>
    private static void ConfigureLogtoResource(
        dynamic options,
        string? logtoResource)
    {
        if (!string.IsNullOrEmpty(logtoResource))
        {
            options.Resource = logtoResource;
            options.Scopes.Add(ReadTodosScope);
            
            Console.WriteLine($"[Web] ✅ API Resource configured: {logtoResource} (will receive JWT access tokens)");
            Console.WriteLine($"[Web] ✅ Requesting scope: {ReadTodosScope}");
        }
        else
        {
            Console.WriteLine("[Web] ⚠️ WARNING: No API Resource configured - will receive OPAQUE access tokens");
            Console.WriteLine("[Web] ⚠️ This will cause API calls to fail!");
            Console.WriteLine("[Web] ⚠️ Configure Logto:Resource in appsettings.json or environment variable");
            Console.WriteLine("[Web] ⚠️ Example: Logto__Resource=https://api.yourdomain.com");
        }
    }

    /// <summary>
    /// Configures OpenID Connect options for Logto.
    /// </summary>
    private static void ConfigureOpenIdConnectOptions(
        IServiceCollection services,
        IHostEnvironment environment)
    {
        services.Configure<OpenIdConnectOptions>(LogtoScheme, options =>
        {
            Console.WriteLine($"[Web] Configuring OpenID Connect '{LogtoScheme}' scheme to save tokens...");
            
            options.SaveTokens = true;
            ConfigureCookieSettings(options, environment);
            
            Console.WriteLine($"[Web] OpenID Connect callback path: {options.CallbackPath}");
            Console.WriteLine($"[Web] Cookie SecurePolicy: {(environment.IsDevelopment() ? "None (dev - allows HTTP)" : "Always (prod - HTTPS required)")}");
            Console.WriteLine($"[Web] Cookie SameSite: {(environment.IsDevelopment() ? "Lax (dev)" : "None (prod - required for OAuth)")}");
            
            ConfigureOpenIdConnectEvents(options);
            
            Console.WriteLine("[Web] OpenID Connect configured to save tokens for API authentication");
        });
        
        services.ConfigureAll<OpenIdConnectOptions>(options =>
        {
            if (!options.SaveTokens)
            {
                Console.WriteLine("[Web] Configuring OpenIdConnect scheme to SaveTokens (fallback for all schemes)");
                options.SaveTokens = true;
            }
        });
    }

    /// <summary>
    /// Configures cookie settings for OAuth flow.
    /// </summary>
    private static void ConfigureCookieSettings(
        OpenIdConnectOptions options,
        IHostEnvironment environment)
    {
        var sameSiteMode = environment.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None;
        var securePolicy = environment.IsDevelopment() 
            ? CookieSecurePolicy.None 
            : CookieSecurePolicy.Always;
        
        options.CorrelationCookie.SameSite = sameSiteMode;
        options.CorrelationCookie.SecurePolicy = securePolicy;
        options.CorrelationCookie.HttpOnly = true;
        
        options.NonceCookie.SameSite = sameSiteMode;
        options.NonceCookie.SecurePolicy = securePolicy;
        options.NonceCookie.HttpOnly = true;
    }

    /// <summary>
    /// Configures OpenID Connect event handlers.
    /// </summary>
    private static void ConfigureOpenIdConnectEvents(OpenIdConnectOptions options)
    {
        var existingOnRemoteFailure = options.Events.OnRemoteFailure;
        options.Events.OnRemoteFailure = context => HandleRemoteFailure(context, existingOnRemoteFailure);
        
        var existingOnTokenValidated = options.Events.OnTokenValidated;
        options.Events.OnTokenValidated = context => HandleTokenValidated(context, existingOnTokenValidated);
    }

    /// <summary>
    /// Handles remote authentication failures.
    /// </summary>
    private static async Task HandleRemoteFailure(
        RemoteFailureContext context,
        Func<RemoteFailureContext, Task>? existingHandler)
    {
        Console.WriteLine(LogSeparator);
        Console.WriteLine("[Web] OnRemoteFailure - Authentication error occurred");
        Console.WriteLine($"[Web] Error: {context.Failure?.Message}");
        Console.WriteLine($"[Web] Error Type: {context.Failure?.GetType().Name}");
        Console.WriteLine($"[Web] Request Path: {context.Request.Path}");
        Console.WriteLine($"[Web] Request Host: {context.Request.Host}");
        Console.WriteLine($"[Web] Request Scheme: {context.Request.Scheme}");
        Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
        Console.WriteLine($"[Web] Cookies Received: {string.Join(", ", context.Request.Cookies.Keys)}");
        Console.WriteLine($"[Web] Cookies Count: {context.Request.Cookies.Count}");

        var correlationCookie = context.Request.Cookies.Keys.FirstOrDefault(k => k.Contains("Correlation"));
        var nonceCookie = context.Request.Cookies.Keys.FirstOrDefault(k => k.Contains("Nonce"));
        Console.WriteLine($"[Web] Correlation Cookie Present: {correlationCookie is not null} ({correlationCookie ?? "not found"})");
        Console.WriteLine($"[Web] Nonce Cookie Present: {nonceCookie is not null} ({nonceCookie ?? "not found"})");

        if (context.Request.Query.Count > 0)
        {
            Console.WriteLine($"[Web] Query Parameters: {string.Join(", ", context.Request.Query.Keys)}");
        }

        if (context.Request.Query.ContainsKey("error"))
        {
            Console.WriteLine($"[Web] OAuth Error: {context.Request.Query["error"]}");
            Console.WriteLine($"[Web] OAuth Error Description: {context.Request.Query["error_description"]}");
        }

        var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
        Console.WriteLine($"[Web] ⚠️ CALLBACK URL: {callbackUrl}");
        Console.WriteLine("[Web] ⚠️ Make sure this EXACT URL is in Logto's 'Redirect URIs' list!");
        Console.WriteLine(LogSeparator);
        
        if (existingHandler is not null)
        {
            await existingHandler(context);
        }
        
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(AuthenticationErrorHtml);
            context.HandleResponse();
        }
    }

    /// <summary>
    /// Handles token validation events.
    /// </summary>
    private static async Task HandleTokenValidated(
        TokenValidatedContext context,
        Func<TokenValidatedContext, Task>? existingHandler)
    {
        Console.WriteLine("[Web] OnTokenValidated event fired - saving tokens to authentication properties");

        if (existingHandler is not null)
        {
            await existingHandler(context);
        }

        if (context.TokenEndpointResponse is not null)
        {
            var hasAccessToken = !string.IsNullOrEmpty(context.TokenEndpointResponse.AccessToken);
            var hasIdToken = !string.IsNullOrEmpty(context.TokenEndpointResponse.IdToken);
            var hasRefreshToken = !string.IsNullOrEmpty(context.TokenEndpointResponse.RefreshToken);

            Console.WriteLine($"[Web] Tokens received - AccessToken: {hasAccessToken}, IdToken: {hasIdToken}, RefreshToken: {hasRefreshToken}");

            if (hasAccessToken)
            {
                Console.WriteLine($"[Web] AccessToken length: {context.TokenEndpointResponse.AccessToken?.Length ?? 0}");
            }
        }

        Console.WriteLine("[Web] Tokens should now be available via HttpContext.GetTokenAsync()");
    }

    /// <summary>
    /// HTML content for authentication error page.
    /// </summary>
    private const string AuthenticationErrorHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>Authentication Error</title>
    <style>
        body { font-family: system-ui; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; background: #1a1a2e; color: white; }
        .error-box { background: #16213e; padding: 40px; border-radius: 10px; text-align: center; max-width: 500px; }
        h1 { color: #e94560; }
        a { color: #0f4c75; background: white; padding: 10px 20px; border-radius: 5px; text-decoration: none; display: inline-block; margin-top: 20px; }
    </style>
</head>
<body>
    <div class='error-box'>
        <h1>Authentication Failed</h1>
        <p>The browser didn't send the required cookies. This typically happens when:</p>
        <ul style='text-align: left;'>
            <li>Using a self-signed certificate on mobile</li>
            <li>Browser security settings block cookies</li>
        </ul>
        <p><strong>Solution:</strong> Use ngrok for a trusted HTTPS connection, or access from localhost on your PC.</p>
        <a href='/dashboard'>Go to Dashboard (without auth)</a>
    </div>
</body>
</html>";

    /// <summary>
    /// Maps authentication endpoints (sign in, sign out) for the web application.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">The configuration to get Logto endpoint</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app, IConfiguration configuration)
    {
        // Sign in endpoint - triggers authentication challenge
        app.MapGet("/auth/signin", async context =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] /auth/signin endpoint called");
            Console.WriteLine($"[Web] User authenticated: {context.User?.Identity?.IsAuthenticated ?? false}");
            Console.WriteLine($"[Web] User name: {context.User?.Identity?.Name ?? "null"}");
            Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            Console.WriteLine($"[Web] Existing cookies: {string.Join(", ", context.Request.Cookies.Keys)}");

            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                Console.WriteLine("[Web] User NOT authenticated - calling ChallengeAsync to redirect to Logto");
                Console.WriteLine("[Web] Redirect URI after login: /");
                Console.WriteLine($"[Web] Expected callback URL: {context.Request.Scheme}://{context.Request.Host}/signin-oidc");

                try
                {
                    await context.ChallengeAsync(new AuthenticationProperties { RedirectUri = "/" });
                    Console.WriteLine("[Web] ✅ ChallengeAsync completed - should redirect to Logto now");
                    Console.WriteLine("[Web] ⚠️ IMPORTANT: Make sure your Logto application has these redirect URIs configured:");
                    Console.WriteLine($"[Web]    - {context.Request.Scheme}://{context.Request.Host}/signin-oidc");
                    Console.WriteLine($"[Web]    - {context.Request.Scheme}://{context.Request.Host}/Callback");
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

            Console.WriteLine(LogSeparator);
        });

        // Sign out endpoint - Full Logto sign-out (signs out from both app and Logto IdP)
        app.MapGet("/auth/signout", async (HttpContext context, IConfiguration config) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] SignOut endpoint called - FULL LOGTO SIGN-OUT");
            Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            Console.WriteLine($"[Web] User authenticated: {context.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"[Web] User name: {context.User?.Identity?.Name}");
            
            // Get the id_token from the authentication properties if available
            var authenticateResult = await context.AuthenticateAsync(LogtoScheme);
            string? idToken = authenticateResult?.Properties?.GetTokenValue("id_token");
            
            Console.WriteLine($"[Web] ID Token available: {idToken is not null}");
            
            // Clear local authentication cookies first
            await context.SignOutAsync(LogtoCookieScheme);
            Console.WriteLine($"[Web] ✅ Cleared {LogtoCookieScheme}");
            
            // Build Logto's end session URL
            var logtoEndpoint = config[LogtoEndpointKey];
            
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
            
            Console.WriteLine("[Web] ➡️  Redirecting to Logto end session:");
            Console.WriteLine($"[Web] {endSessionUrl}");
            Console.WriteLine(LogSeparator);
            
            // Redirect to Logto's end session endpoint
            context.Response.Redirect(endSessionUrl);
        }).AllowAnonymous();

        // Logout complete callback - where Logto redirects back after sign-out
        app.MapGet("/logout-complete", async (HttpContext context) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] Logout complete callback - user returned from Logto");
            Console.WriteLine($"[Web] Current authentication state: {context.User?.Identity?.IsAuthenticated}");
            
            // Make absolutely sure cookies are cleared
            try
            {
                await context.SignOutAsync(LogtoCookieScheme);
                await context.SignOutAsync(LogtoScheme);
                Console.WriteLine("[Web] ✅ Cleared all authentication cookies");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Web] Cookie clear error (might be already cleared): {ex.Message}");
            }
            
            // Return HTML that uses JavaScript to navigate to login
            // This ensures a complete page reload and new Blazor circuit
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(LogoutCompleteHtml);
            Console.WriteLine("[Web] ✅ Sent HTML redirect to /login");
            Console.WriteLine(LogSeparator);
        }).AllowAnonymous();

        // Simple manual sign-out endpoint for debugging (bypasses Logto's end session endpoint)
        // This just clears local cookies without going to Logto
        app.MapGet("/auth/signout/local", async (HttpContext context) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] Local sign-out endpoint called (bypassing Logto end session)");
            Console.WriteLine($"[Web] User was: {context.User?.Identity?.Name ?? "unknown"}");
            
            try
            {
                // Clear local authentication cookies
                await context.SignOutAsync(LogtoCookieScheme);
                Console.WriteLine($"[Web] ✅ Cleared {LogtoCookieScheme}");
                
                // Return HTML that forces a complete page reload to /login
                // This ensures Blazor circuit is completely reset
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(LocalSignOutHtml);
                Console.WriteLine("[Web] ✅ Sent HTML redirect to /login (forced reload)");
                Console.WriteLine(LogSeparator);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Web] ⚠️ ERROR during local sign-out: {ex.Message}");
                Console.WriteLine(LogSeparator);
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

