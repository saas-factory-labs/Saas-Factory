using System.Security.Claims;
using AppBlueprint.Infrastructure.Configuration;
using AppBlueprint.Infrastructure.DatabaseContexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppBlueprint.Infrastructure.Authentication;

/// <summary>
/// Lightweight DTO for looking up user's tenant during authentication.
/// </summary>
internal sealed record UserTenantLookup(string Id, string TenantId);

/// <summary>
/// Extension methods for configuring web authentication (Logto, cookies, data protection)
/// </summary>
public static class WebAuthenticationExtensions
{
    // Static readonly arrays for test endpoint configuration (CA1861)
    private static readonly string[] RequiredRedirectUris = new[]
    {
        "https://appblueprint-web-staging.up.railway.app/callback",
        "https://appblueprint-web-staging.up.railway.app/signout-callback-logto"
    };

    private static readonly string[] RequiredPostLogoutRedirectUris = new[]
    {
        "https://appblueprint-web-staging.up.railway.app/"
    };
    // Constants for authentication schemes
    private const string LogtoCookieScheme = "Logto.Cookie";
    private const string LogtoScheme = "Logto";
    private const string FirebaseCookieScheme = "Firebase.Cookie";
    private const string FirebaseLoginPath = "/signin/firebase";

    // Constants for configuration keys
    private const string LogtoEndpointKey = "Logto:Endpoint";
    private const string LogtoAppIdKey = "Logto:AppId";
    private const string LogtoAppSecretKey = "Logto:AppSecret";
    private const string LogtoResourceKey = "Logto:Resource";

    // Constants for paths and URIs
    private const string DataProtectionKeysPath = "/app";
    private const string DataProtectionKeysFolderName = "DataProtection-Keys";
    private const string ApplicationName = "AppBlueprint";
    private const string SignupPath = "/signup";
    private const string SignoutPath = "/auth/signout";
    private const string NullPlaceholder = "(null)";

    // Constants for logging
    private const string LogSeparator = "========================================";

    // Internal static class to hold HTML templates
    private static class HtmlTemplates
    {
        public const string LogoutCompleteHtml = @"
<!DOCTYPE html>
<html>
<head>
    <title>Signing out...</title>
    <meta http-equiv='refresh' content='0; url=/login'>
    <meta http-equiv='cache-control' content='no-cache, no-store, must-revalidate'>
    <meta http-equiv='pragma' content='no-cache'>
    <meta http-equiv='expires' content='0'>
    <script>
        console.log('[Logout] Redirecting to login page after sign-out');
        // Use replace() instead of href to prevent browser back button issues
        window.location.replace('/login');
    </script>
</head>
<body>
    <p>Signing out... You will be redirected to the login page.</p>
    <p>If not redirected automatically, <a href='/login'>click here</a>.</p>
</body>
</html>";

        public const string LocalSignOutHtml = @"
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
    }

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

        bool hasFirebaseConfig = IsFirebaseConfigured(configuration);
        bool hasLogtoConfig = ConfigurationValidator.ValidateLogtoConfiguration(configuration, throwOnMissing: false);

        if (hasFirebaseConfig)
        {
            ConfigureFirebaseAuthentication(services, environment);
        }
        else if (hasLogtoConfig)
        {
            ConfigureLogtoAuthentication(services, configuration, environment);
        }
        else
        {
            // In production, authentication must be fully configured.
            // Silently falling back to a bare Cookies scheme causes a runtime crash
            // the first time any endpoint challenges with the "Logto" scheme.
            // Failing fast at startup produces a clear error in the deployment logs.
            if (!environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "Authentication is not configured. " +
                    "Set the following environment variables in your Railway web service:\n" +
                    "  LOGTO_ENDPOINT   — e.g. https://32nkyp.logto.app\n" +
                    "  LOGTO_APPID      — Logto Application ID\n" +
                    "  LOGTO_APPSECRET  — Logto Application Secret\n" +
                    "  LOGTO_APIRESOURCE — (optional) API resource indicator\n" +
                    "These must be set directly on the web service, not only in AppHost.");
            }

            ConfigureFallbackAuthentication(services);
        }

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());

        services.AddScoped<Authorization.ITokenStorageService, Authorization.TokenStorageService>();
        services.AddScoped<Authorization.IAuthenticationTokenService, Authorization.AuthenticationTokenService>();

        return services;
    }

    /// <summary>
    /// Returns true when AUTHENTICATION_PROVIDER=Firebase is configured.
    /// </summary>
    private static bool IsFirebaseConfigured(IConfiguration configuration)
    {
        string? provider = Environment.GetEnvironmentVariable("AUTHENTICATION_PROVIDER")
                        ?? configuration["Authentication:Provider"];
        return string.Equals(provider, "Firebase", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Configures cookie-based authentication for Firebase (no OIDC redirect; login is handled
    /// by FirebaseAuthController POSTing to /auth/firebase-signin).
    /// </summary>
    private static void ConfigureFirebaseAuthentication(IServiceCollection services, IHostEnvironment environment)
    {
        Console.WriteLine("[Web] Firebase authentication mode detected");
        Console.WriteLine($"[Web] Login path: {FirebaseLoginPath}");

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = FirebaseCookieScheme;
            options.DefaultChallengeScheme = FirebaseCookieScheme;
            options.DefaultSignInScheme = FirebaseCookieScheme;
            options.DefaultAuthenticateScheme = FirebaseCookieScheme;
            options.DefaultSignOutScheme = FirebaseCookieScheme;
        })
        .AddCookie(FirebaseCookieScheme, options =>
        {
            options.LoginPath = FirebaseLoginPath;
            options.LogoutPath = SignoutPath;
            options.AccessDeniedPath = FirebaseLoginPath;

            if (environment.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }

            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.Cookie.IsEssential = true;

            Console.WriteLine($"[Web] Configured Firebase.Cookie scheme: LoginPath={options.LoginPath}");
        });

        // Register the lightweight Firebase sign-in service (no JSInterop dependency)
        services.AddScoped<IFirebaseSignInService, FirebaseSignInService>();

        Console.WriteLine("[Web] Firebase authentication configured successfully");
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
        // Read from flat UPPERCASE environment variables first, then fall back to IConfiguration
        // Supports: LOGTO_APPID, LOGTO_ENDPOINT, LOGTO_APPSECRET, LOGTO_RESOURCE (flat standard)
        // Falls back to: Logto:AppId, Logto:Endpoint, Logto:AppSecret, Logto:Resource (hierarchical)
        string? logtoAppId = Environment.GetEnvironmentVariable("LOGTO_APPID")
                          ?? Environment.GetEnvironmentVariable("LOGTO_APP_ID")
                          ?? configuration[LogtoAppIdKey];
        string? logtoEndpoint = Environment.GetEnvironmentVariable("LOGTO_ENDPOINT")
                             ?? configuration[LogtoEndpointKey];
        string? logtoAppSecret = Environment.GetEnvironmentVariable("LOGTO_APPSECRET")
                              ?? Environment.GetEnvironmentVariable("LOGTO_APP_SECRET")
                              ?? configuration[LogtoAppSecretKey];
        string? logtoResource = Environment.GetEnvironmentVariable("LOGTO_APIRESOURCE")
                             ?? Environment.GetEnvironmentVariable("LOGTO_RESOURCE")
                             ?? configuration[LogtoResourceKey];

        // DEBUG: Check all configuration values
        Console.WriteLine($"[Web] DEBUG - Reading configuration:");
        Console.WriteLine($"[Web] DEBUG - {LogtoEndpointKey} = {logtoEndpoint ?? NullPlaceholder}");
        Console.WriteLine($"[Web] DEBUG - {LogtoAppIdKey} = {logtoAppId ?? NullPlaceholder}");
        Console.WriteLine($"[Web] DEBUG - {LogtoAppSecretKey} = {(string.IsNullOrEmpty(logtoAppSecret) ? "(null or empty)" : $"<SET, length={logtoAppSecret.Length}>")} ");
        Console.WriteLine($"[Web] DEBUG - {LogtoResourceKey} = {logtoResource ?? NullPlaceholder}");
        Console.WriteLine($"[Web] DEBUG - Environment var LOGTO_APP_SECRET = {(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOGTO_APP_SECRET")) ? "(null or empty)" : $"<SET, length={Environment.GetEnvironmentVariable("LOGTO_APP_SECRET")?.Length}>")}");
        Console.WriteLine($"[Web] DEBUG - Environment var LOGTO_RESOURCE = {Environment.GetEnvironmentVariable("LOGTO_RESOURCE") ?? NullPlaceholder}");

        LogLogtoConfiguration(logtoEndpoint, logtoAppId, logtoAppSecret, logtoResource);

        // AddLogtoAuthentication ONLY registers the cookie scheme, not OIDC
        // We need to manually add the OpenID Connect handler with the "Logto" scheme
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = LogtoCookieScheme;
            options.DefaultChallengeScheme = LogtoScheme;
            options.DefaultSignInScheme = LogtoCookieScheme;
            options.DefaultAuthenticateScheme = LogtoCookieScheme;
            options.DefaultSignOutScheme = LogtoScheme; // OIDC scheme handles full sign-out including Logto session
        })
        .AddCookie(LogtoCookieScheme, options =>
        {
            options.LoginPath = SignupPath;
            options.LogoutPath = SignoutPath;
            options.AccessDeniedPath = SignupPath;

            if (environment.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.Cookie.IsEssential = true;

            Console.WriteLine($"[Web] Configured Cookie scheme: LoginPath={options.LoginPath}");
        })
        .AddOpenIdConnect(LogtoScheme, options =>
        {
            // Normalize the endpoint so the OIDC authority always ends with /oidc.
            // Accepts both https://tenant.logto.app and https://tenant.logto.app/oidc
            // so the Doppler/env value can be set either way without double-appending.
            string? oidcBase = NormalizeLogtoOidcEndpoint(logtoEndpoint);
            options.Authority = oidcBase;
            options.MetadataAddress = string.IsNullOrEmpty(oidcBase)
                ? null
                : $"{oidcBase}/.well-known/openid-configuration";
            options.ClientId = logtoAppId ?? string.Empty;
            options.ClientSecret = logtoAppSecret;

            options.ResponseType = "code";
            options.ResponseMode = "query";

            options.SaveTokens = true;

            // IMPORTANT: When using API Resources, the access token is audience-restricted
            // to the API and cannot be used to call the /userinfo endpoint (returns 401).
            // We get user claims from the ID token instead.
            // Only enable userinfo endpoint when NOT using API Resources.
            options.GetClaimsFromUserInfoEndpoint = string.IsNullOrEmpty(logtoResource);

            // Configure scopes - include OIDC scopes and API resource scopes
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("offline_access"); // Required for refresh tokens

            // Configure API Resource to request JWT access tokens
            // Per Logto docs: https://docs.logto.io/authorization/global-api-resources
            // 1. Set the resource indicator
            // 2. Add the API resource scopes to the Scope collection
            // 3. Create a Role in Logto Console and assign permissions to it
            // 4. Assign the Role to users
            if (!string.IsNullOrEmpty(logtoResource))
            {
                options.Resource = logtoResource; // Request access token for this API resource

                // Add API resource scopes - these must be defined in Logto Console
                // under API Resources -> Your API -> Permissions
                // AND assigned to a Role -> assigned to the user
                options.Scope.Add("read:files");
                options.Scope.Add("write:files");
                options.Scope.Add("read:todos");

                Console.WriteLine($"[Web] ✅ API Resource configured: {logtoResource} (will receive JWT access tokens)");
                Console.WriteLine("[Web] ✅ Requesting scopes: openid profile email offline_access read:files write:files read:todos");
                Console.WriteLine("[Web] ⚠️ IMPORTANT: Create a Role in Logto Console, assign these permissions to it, then assign the Role to users!");
            }
            else
            {
                Console.WriteLine("[Web] ⚠️ WARNING: No API Resource configured - will receive OPAQUE access tokens");
            }

            options.CallbackPath = "/callback";
            options.SignedOutCallbackPath = "/signout-callback-logto";

            if (environment.IsDevelopment())
            {
                options.RequireHttpsMetadata = false;
            }

            // Configure correlation cookie for development (HTTP localhost)
            options.CorrelationCookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.None
                : CookieSecurePolicy.Always;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.HttpOnly = true;
            options.CorrelationCookie.IsEssential = true;
            options.CorrelationCookie.Path = "/";

            // Configure nonce cookie for development (HTTP localhost)
            options.NonceCookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.None
                : CookieSecurePolicy.Always;
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.NonceCookie.HttpOnly = true;
            options.NonceCookie.IsEssential = true;
            options.NonceCookie.Path = "/";

            Console.WriteLine($"[Web] Configured OpenID Connect 'Logto' scheme with Authority={options.Authority}");
            Console.WriteLine($"[Web] Cookie settings - SecurePolicy: {options.CorrelationCookie.SecurePolicy}, SameSite: {options.CorrelationCookie.SameSite}");
        });

        // Configure the Logto cookie scheme (named "Logto.Cookie") specifically
        services.Configure<CookieAuthenticationOptions>("Logto.Cookie", options =>
        {
            // Set the correct login path for Logto authentication
            options.LoginPath = SignupPath;
            options.LogoutPath = SignoutPath;
            options.AccessDeniedPath = SignupPath;

            // Configure authentication cookie for development
            if (environment.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.Cookie.IsEssential = true;

            Console.WriteLine($"[Web] Configured Logto.Cookie scheme: LoginPath={options.LoginPath}, Path=/, IsEssential=true, SameSite={options.Cookie.SameSite}");
        });

        // Also configure all cookie schemes as a fallback
        services.ConfigureAll<CookieAuthenticationOptions>(options =>
        {
            // Set the correct login path for Logto authentication
            options.LoginPath = SignupPath;
            options.LogoutPath = SignoutPath;
            options.AccessDeniedPath = SignupPath;

            // Configure authentication cookie for development
            if (environment.IsDevelopment())
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.SameSite = SameSiteMode.Lax;
            }
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = "/";
            options.Cookie.IsEssential = true;

            Console.WriteLine($"[Web] Configured authentication cookie: LoginPath={options.LoginPath}, Path=/, IsEssential=true, SameSite={options.Cookie.SameSite}");
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
        Console.WriteLine("[Web]   - LOGTO_APPID");
        Console.WriteLine("[Web]   - LOGTO_ENDPOINT");
        Console.WriteLine("[Web]   - LOGTO_APPSECRET");
        Console.WriteLine($"[Web] {LogSeparator}");

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = SignupPath;
                options.LogoutPath = SignoutPath;
                options.AccessDeniedPath = SignupPath;
            });
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

            // Logto uses /callback as the callback path (not the default /signin-oidc)
            // This must match what's configured in Logto's redirect URIs
            options.CallbackPath = "/callback";

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
        // In development (HTTP localhost), use Lax + None for cookies
        // In production (HTTPS), use None + Always (required for OAuth cross-site redirects)
        SameSiteMode sameSiteMode;
        CookieSecurePolicy securePolicy;

        if (environment.IsDevelopment())
        {
            // Development: HTTP localhost
            sameSiteMode = SameSiteMode.Lax;
            securePolicy = CookieSecurePolicy.None; // Allow HTTP
        }
        else
        {
            // Production: HTTPS required
            sameSiteMode = SameSiteMode.None;
            securePolicy = CookieSecurePolicy.Always;
        }

        options.CorrelationCookie.SameSite = sameSiteMode;
        options.CorrelationCookie.SecurePolicy = securePolicy;
        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.Path = "/";
        options.CorrelationCookie.IsEssential = true;
        options.CorrelationCookie.Domain = null; // Don't set domain - use current host

        options.NonceCookie.SameSite = sameSiteMode;
        options.NonceCookie.SecurePolicy = securePolicy;
        options.NonceCookie.HttpOnly = true;
        options.NonceCookie.Path = "/";
        options.NonceCookie.IsEssential = true;
        options.NonceCookie.Domain = null; // Don't set domain - use current host

        Console.WriteLine($"[Web] Configured correlation/nonce cookies: Path=/, IsEssential=true, SameSite={sameSiteMode}, Secure={securePolicy}, Domain=null");
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

        // Add resource parameter to authorization request to get JWT tokens
        // Per Logto docs: https://docs.logto.io/authorization/global-api-resources
        // The 'resource' parameter tells Logto which API Resource the token is for
        options.Events.OnRedirectToIdentityProvider = context =>
        {
            Console.WriteLine("[Web] OnRedirectToIdentityProvider - About to redirect to Logto");
            Console.WriteLine($"[Web] Redirect URL: {context.ProtocolMessage.RedirectUri}");

            // CRITICAL: Add the 'resource' parameter to the authorization request
            // This tells Logto to issue a JWT access token instead of an opaque token
            string? resource = context.Options.Resource;
            if (!string.IsNullOrEmpty(resource))
            {
                context.ProtocolMessage.SetParameter("resource", resource);
                Console.WriteLine($"[Web] ✅ Added 'resource' parameter: {resource}");
                Console.WriteLine("[Web] ✅ Logto should now issue a JWT access token with scopes");
            }
            else
            {
                Console.WriteLine("[Web] ⚠️ WARNING: No 'resource' parameter set - will receive opaque token");
                Console.WriteLine("[Web] ⚠️ Set LOGTO_RESOURCE environment variable to your API Resource identifier");
            }

            Console.WriteLine($"[Web] Response cookies being set: {context.Response.Headers.ContainsKey("Set-Cookie")}");

            if (context.Response.Headers.TryGetValue("Set-Cookie", out var cookies))
            {
                Console.WriteLine($"[Web] Set-Cookie headers count: {cookies.Count}");
                foreach (var cookie in cookies)
                {
                    // Only log cookie names, not values
                    string? cookieStr = cookie;
                    if (!string.IsNullOrEmpty(cookieStr))
                    {
                        string cookieName = cookieStr.Split('=')[0];
                        Console.WriteLine($"[Web]   - Setting cookie: {cookieName}");
                    }
                }
            }

            return Task.CompletedTask;
        };

        // CRITICAL: Add resource parameter to the token exchange request
        // Per Logto docs: The resource parameter must be sent in the token request to get a JWT
        // https://docs.logto.io/authorization/global-api-resources#authorization-code-or-refresh-token-flow
        options.Events.OnAuthorizationCodeReceived = context =>
        {
            Console.WriteLine("[Web] OnAuthorizationCodeReceived - About to exchange code for tokens");

            // Add the 'resource' parameter to the token request
            // This tells Logto to issue a JWT access token instead of an opaque token
            string? resource = context.Options.Resource;
            if (!string.IsNullOrEmpty(resource))
            {
                // Add resource to the token request parameters
                context.TokenEndpointRequest?.SetParameter("resource", resource);
                Console.WriteLine($"[Web] ✅ Added 'resource' parameter to token request: {resource}");
                Console.WriteLine("[Web] ✅ Logto should now return a JWT access token (length ~800+)");
            }
            else
            {
                Console.WriteLine("[Web] ⚠️ WARNING: No 'resource' parameter - will receive opaque token (length ~43)");
            }

            return Task.CompletedTask;
        };

        // Debug: Log when callback is received
        options.Events.OnMessageReceived = context =>
        {
            Console.WriteLine("[Web] OnMessageReceived - Callback received from Logto");
            Console.WriteLine($"[Web] Request URL: {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            Console.WriteLine($"[Web] Request cookies count: {context.Request.Cookies.Count}");

            foreach (var cookie in context.Request.Cookies)
            {
                Console.WriteLine($"[Web]   - Received cookie: {cookie.Key}");
            }

            // Check for correlation cookie specifically
            bool hasCorrelation = context.Request.Cookies.Any(c => c.Key.Contains("Correlation", StringComparison.Ordinal));
            bool hasNonce = context.Request.Cookies.Any(c => c.Key.Contains("Nonce", StringComparison.Ordinal));

            Console.WriteLine($"[Web] Has Correlation cookie: {hasCorrelation}");
            Console.WriteLine($"[Web] Has Nonce cookie: {hasNonce}");

            if (!hasCorrelation || !hasNonce)
            {
                Console.WriteLine("[Web] ⚠️ WARNING: Missing required cookies! This will cause authentication to fail.");
                Console.WriteLine("[Web] This typically means:");
                Console.WriteLine("[Web]   1. Browser blocked third-party cookies");
                Console.WriteLine("[Web]   2. Cookie SameSite policy is too restrictive");
                Console.WriteLine("[Web]   3. Redirect from Logto is treated as cross-site");
            }

            return Task.CompletedTask;
        };
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

        var correlationCookie = context.Request.Cookies.Keys.FirstOrDefault(k => k.Contains("Correlation", StringComparison.Ordinal));
        var nonceCookie = context.Request.Cookies.Keys.FirstOrDefault(k => k.Contains("Nonce", StringComparison.Ordinal));
        Console.WriteLine($"[Web] Correlation Cookie Present: {correlationCookie is not null} ({correlationCookie ?? "not found"})");
        Console.WriteLine($"[Web] Nonce Cookie Present: {nonceCookie is not null} ({nonceCookie ?? "not found"})");

        if (context.Request.Query.Count > 0)
        {
            Console.WriteLine($"[Web] Query Parameters: {string.Join(", ", context.Request.Query.Keys)}");
        }

        if (context.Request.Query.TryGetValue("error", out var error))
        {
            Console.WriteLine($"[Web] OAuth Error: {error}");
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
            context.Response.StatusCode = 500;
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

        // DEBUG: Log all claims from OIDC token
        if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
        {
            Console.WriteLine($"[Web] Claims from OIDC token ({claimsIdentity.Claims.Count()} claims):");
            foreach (var claim in claimsIdentity.Claims)
            {
                string preview = claim.Value.Length > 50 ? string.Concat(claim.Value.AsSpan(0, Math.Min(50, claim.Value.Length)), "...") : claim.Value;
                Console.WriteLine($"[Web]   - {claim.Type}: {preview}");
            }
        }

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

        // Fetch user's tenant from database and add tenant_id claim
        string? userEmail = context.Principal?.FindFirst(ClaimTypes.Email)?.Value
            ?? context.Principal?.FindFirst("email")?.Value;

        if (!string.IsNullOrEmpty(userEmail))
        {
            Console.WriteLine($"[Web] User email from JWT: {userEmail}");

            // Get DbContextFactory from DI container to bypass RLS
            var contextFactory = context.HttpContext.RequestServices.GetService<IDbContextFactory<ApplicationDbContext>>();

            if (contextFactory is not null)
            {
                await using ApplicationDbContext dbContext = await contextFactory.CreateDbContextAsync();

                // Query database directly, bypassing RLS by using SECURITY INVOKER context
                // This is safe because we're only reading data during authentication
                // Use quoted column names to match exact casing in PostgreSQL
                FormattableString query = $"SELECT \"Id\", \"TenantId\" FROM \"Users\" WHERE \"Email\" = {userEmail} LIMIT 1";

                var result = await dbContext.Database.SqlQuery<UserTenantLookup>(query).FirstOrDefaultAsync();

                if (result is not null && !string.IsNullOrEmpty(result.TenantId))
                {
                    Console.WriteLine($"[Web] ✅ Found user's tenant: {result.TenantId}");

                    // Add tenant_id claim to ClaimsIdentity
                    // IMPORTANT: We need to add it to the existing identity that will be saved to the cookie
                    var identity = context.Principal?.Identity as ClaimsIdentity;
                    if (identity is not null)
                    {
                        identity.AddClaim(new Claim("tenant_id", result.TenantId));
                        Console.WriteLine("[Web] ✅ Added tenant_id claim to ClaimsIdentity");

                        // CRITICAL: Create a new ClaimsPrincipal with the updated identity
                        // This ensures the claim is persisted to the authentication cookie
                        context.Principal = new ClaimsPrincipal(identity);
                        Console.WriteLine("[Web] ✅ Updated Principal with tenant_id claim");
                    }

                    // User has a tenant - redirect directly to dashboard (skip onboarding)
                    if (context.Properties is not null)
                    {
                        context.Properties.RedirectUri = "/dashboard";
                        Console.WriteLine("[Web] ✅ User has tenant - redirecting to /dashboard");
                    }
                }
                else
                {
                    Console.WriteLine("[Web] ⚠️ User not found or has no tenant - will redirect to onboarding");

                    // User has no tenant - redirect to onboarding for profile completion
                    if (context.Properties is not null)
                    {
                        context.Properties.RedirectUri = "/onboarding";
                        Console.WriteLine("[Web] ⚠️ Redirecting to /onboarding for profile completion");
                    }
                }
            }
            else
            {
                Console.WriteLine("[Web] ⚠️ DbContextFactory not available in DI container");
                // Fallback to onboarding if we can't check database
                context.Properties?.RedirectUri = "/onboarding";
            }
        }
        else
        {
            Console.WriteLine("[Web] ⚠️ No email claim found in JWT");
            // Fallback to onboarding if we can't identify user
            context.Properties?.RedirectUri = "/onboarding";
        }

        Console.WriteLine("[Web] Tokens should now be available via HttpContext.GetTokenAsync()");
    }

    /// <summary>
    /// Maps authentication endpoints (sign in, sign out) for the web application.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="configuration">The configuration to get Logto endpoint</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapAuthenticationEndpoints(this WebApplication app, IConfiguration configuration)
    {
        MapSignInEndpoint(app);
        MapSignOutEndpoint(app);
        MapLogoutCompleteEndpoint(app);
        MapLocalSignOutEndpoint(app);

        return app;
    }

    private static void MapSignInEndpoint(WebApplication app)
    {
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
                Console.WriteLine($"[Web] Expected callback URL: {context.Request.Scheme}://{context.Request.Host}/callback");

                try
                {
                    // Explicitly challenge with the Logto scheme so the OIDC middleware
                    // redirects the browser to the Logto authorization endpoint.
                    // Using the explicit scheme name is more reliable than relying on
                    // DefaultChallengeScheme resolution, which can vary by runtime context.
                    await context.ChallengeAsync(LogtoScheme, new AuthenticationProperties { RedirectUri = "/" });
                    Console.WriteLine("[Web] ✅ ChallengeAsync(LogtoScheme) completed - should redirect to Logto now");
                    Console.WriteLine("[Web] ⚠️ IMPORTANT: Make sure your Logto application has this redirect URI configured:");
                    Console.WriteLine($"[Web]    - {context.Request.Scheme}://{context.Request.Host}/callback");
                    Console.WriteLine(LogSeparator);
                    return;
                }
                catch (Exception ex)
                {
                    // Log full details server-side only — never expose exception details to the client
                    Console.WriteLine($"[Web] ❌ ERROR in ChallengeAsync: {ex.GetType().Name}");
                    Console.WriteLine($"[Web] ❌ Message: {ex.Message}");
                    if (ex.InnerException is not null)
                    {
                        Console.WriteLine($"[Web] ❌ Inner exception: {ex.InnerException.GetType().Name}");
                        Console.WriteLine($"[Web] ❌ Inner message: {ex.InnerException.Message}");
                    }
                    Console.WriteLine("[Web] ❌ Check LOGTO_ENDPOINT, LOGTO_APPID, LOGTO_APPSECRET env vars are set and the Logto service is reachable.");
                    Console.WriteLine(LogSeparator);

                    // Redirect to signup with an error indicator rather than returning 500.
                    // A 500 gets caught by UseStatusCodePagesWithReExecute and renders the
                    // Blazor <NotFound> template at this URL, which is confusing to users.
                    context.Response.Redirect("/signup?auth-error=provider-unavailable");
                    return;
                }
            }

            // Check if user has completed onboarding (has a tenant_id claim)
            string? tenantId = context.User?.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrEmpty(tenantId))
            {
                Console.WriteLine($"[Web] User already authenticated with tenant {tenantId} - redirecting to /dashboard");
                Console.WriteLine(LogSeparator);
                context.Response.Redirect("/dashboard");
            }
            else
            {
                Console.WriteLine("[Web] User already authenticated but has no tenant - redirecting to /onboarding");
                Console.WriteLine(LogSeparator);
                context.Response.Redirect("/onboarding");
            }
        }).AllowAnonymous();
    }

    private static void MapSignOutEndpoint(WebApplication app)
    {
        app.MapGet("/auth/signout", async (HttpContext context) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] SignOut endpoint called");
            Console.WriteLine($"[Web] User authenticated: {context.User?.Identity?.IsAuthenticated}");

            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                Console.WriteLine("[Web] ✅ Signing out from both Cookie and OIDC schemes");
                Console.WriteLine("[Web] ➡️  Will redirect to /login after sign-out completes");
                Console.WriteLine(LogSeparator);

                await context.SignOutAsync(LogtoCookieScheme);
                await context.SignOutAsync(LogtoScheme, new AuthenticationProperties { RedirectUri = "/login" });
            }
            else
            {
                Console.WriteLine("[Web] User not authenticated - redirecting directly to /login");
                Console.WriteLine(LogSeparator);
                context.Response.Redirect("/login");
            }
        }).AllowAnonymous();
    }

    private static void MapLogoutCompleteEndpoint(WebApplication app)
    {
        app.MapGet("/logout-complete", async (HttpContext context) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] Logout complete callback - user returned from Logto");
            Console.WriteLine($"[Web] Current authentication state: {context.User?.Identity?.IsAuthenticated}");

            try
            {
                await context.SignOutAsync(LogtoCookieScheme);
                await context.SignOutAsync(LogtoScheme);
                Console.WriteLine("[Web] ✅ Cleared all authentication cookies");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[Web] Cookie clear error (might be already cleared): {ex.Message}");
            }

            Console.WriteLine("[Web] ✅ Sending HTML redirect to /login");
            Console.WriteLine(LogSeparator);

            return Results.Content(HtmlTemplates.LogoutCompleteHtml, "text/html", null, statusCode: 200);
        }).AllowAnonymous();
    }

    private static void MapLocalSignOutEndpoint(WebApplication app)
    {
        app.MapGet("/auth/signout/local", async (HttpContext context) =>
        {
            Console.WriteLine(LogSeparator);
            Console.WriteLine("[Web] Local sign-out endpoint called (bypassing Logto end session)");
            Console.WriteLine($"[Web] User was: {context.User?.Identity?.Name ?? "unknown"}");

            try
            {
                await context.SignOutAsync(LogtoCookieScheme);
                Console.WriteLine($"[Web] ✅ Cleared {LogtoCookieScheme}");

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(HtmlTemplates.LocalSignOutHtml);
                Console.WriteLine("[Web] ✅ Sent HTML redirect to /login (forced reload)");
                Console.WriteLine(LogSeparator);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"[Web] ⚠️ ERROR during local sign-out: {ex.Message}");
                Console.WriteLine(LogSeparator);
                context.Response.Redirect("/login");
            }
        }).AllowAnonymous();
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
            List<object> results = [];

            // Test 1: DNS Resolution
            try
            {
                var host = "32nkyp.logto.app";
                var addresses = await System.Net.Dns.GetHostAddressesAsync(host);
                results.Add(new
                {
                    test = "DNS Resolution",
                    success = true,
                    host,
                    addresses = addresses.Select(a => a.ToString()).ToArray()
                });
            }
            catch (System.Net.Sockets.SocketException ex)
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
                var response = await client.GetAsync(new Uri(url, UriKind.Absolute));
                sw.Stop();
                var content = await response.Content.ReadAsStringAsync();

                results.Add(new
                {
                    test = "HTTPS Connectivity",
                    success = response.IsSuccessStatusCode,
                    url,
                    statusCode = (int)response.StatusCode,
                    statusDescription = response.ReasonPhrase,
                    contentLength = content.Length,
                    elapsedMs = sw.ElapsedMilliseconds,
                    warning = sw.ElapsedMilliseconds > 5000 ? "⚠️ Slow connection - took over 5 seconds" : null
                });
            }
            catch (HttpRequestException ex)
            {
                results.Add(new
                {
                    test = "HTTPS Connectivity",
                    success = false,
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
            catch (TaskCanceledException ex)
            {
                results.Add(new
                {
                    test = "HTTPS Connectivity",
                    success = false,
                    error = "Request timed out: " + ex.Message,
                    type = ex.GetType().Name
                });
            }

            // Test 3: Current Logto Configuration
            var logtoConfig = new
            {
                test = "Logto Configuration",
                endpoint = configuration[LogtoEndpointKey],
                appId = configuration[LogtoAppIdKey],
                hasAppSecret = !string.IsNullOrEmpty(configuration[LogtoAppSecretKey]),
                environment = environment.EnvironmentName,
                requiredRedirectUris = RequiredRedirectUris,
                requiredPostLogoutRedirectUris = RequiredPostLogoutRedirectUris,
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

    /// <summary>
    /// Normalizes a Logto endpoint URL so it always ends with exactly one /oidc suffix.
    /// Accepts both https://tenant.logto.app and https://tenant.logto.app/oidc so that
    /// the Doppler/environment value can be set either way without double-appending /oidc.
    /// Returns null/empty unchanged.
    /// </summary>
    public static string? NormalizeLogtoOidcEndpoint(string? endpoint)
    {
        if (string.IsNullOrEmpty(endpoint))
            return endpoint;

        string trimmed = endpoint.TrimEnd('/');
        return trimmed.EndsWith("/oidc", StringComparison.OrdinalIgnoreCase)
            ? trimmed
            : trimmed + "/oidc";
    }
}

