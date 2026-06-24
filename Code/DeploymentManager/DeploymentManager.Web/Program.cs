using AppBlueprint.AdminPortalKernel.Infrastructure;
using AppBlueprint.AdminPortalKernel.PluginLoading;
using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Authentication.Extensions;
using AppBlueprint.Infrastructure.Extensions;
using AppBlueprint.ServiceDefaults;
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Services;
using DeploymentManager.Web.Components;
using DeploymentManager.Web.Services;
// using DeploymentManager.Web.Services.Impersonation; // re-enable with the impersonation DI block below
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Serve static web assets from referenced Razor class libraries in published/container deployments.
builder.WebHost.UseStaticWebAssets();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
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

builder.Services.AddAppBlueprintConfiguration(builder.Configuration, builder.Environment);
builder.Services.AddScoped<ICurrentTenantService, NullCurrentTenantService>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    // OIDC auth cookies (access + refresh + ID tokens + claims) exceed the 32KB default.
    // Raising to 64KB keeps single-instance deploys working; the proper fix is the
    // in-memory ticket store registered below which shrinks the cookie to a session ID.
    options.Limits.MaxRequestHeadersTotalSize = 65_536;
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // Unspecified = don't enforce a minimum - lets SameSite=None on OIDC correlation/nonce
        // cookies pass through unchanged. Setting Lax here would downgrade SameSite=None back
        // to Lax, breaking the cross-site form_post callback from Logto.
        options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        options.Secure = CookieSecurePolicy.None;
        options.CheckConsentNeeded = _ => false;
    });
}

builder.Services.AddOutputCache();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = builder.Environment.IsDevelopment();
});
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddUiKit();
builder.Services.AddHttpContextAccessor();
builder.Services.AddWebAuthentication(builder.Configuration, builder.Environment);

// Store OIDC tickets server-side so the browser cookie is a small GUID instead of
// multi-chunk JWT headers that breach Kestrel's 431 limit.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<InMemoryTicketStore>();
builder.Services.AddOptions<CookieAuthenticationOptions>("Logto.Cookie")
    .PostConfigure<InMemoryTicketStore>((options, store) => options.SessionStore = store);

// DeploymentManager-specific services
builder.Services.AddScoped<IMenuConfigurationService, MenuConfigurationService>();

// Tenant impersonation (act-as) - DISABLED / PARKED (kept for a future decision, not deleted).
//
// WHY DISABLED: the actual requirement is secure, read-only, rate-limited *viewing* of tenant/user
// data (for support + an audit trail), NOT logging in as the user. That is already provided by the
// AdminPortalKernel pipeline registered via AddAdminPortalKernel below: AdminQuerySession ->
// IAdminAccessGuard (role, MFA, rate limit, nonce, ticket, alert, SIEM, audit) -> IAdminAccessRateLimiter
// (caps distinct tenants per admin per rolling hour - the anti-bulk-extraction control), surfaced by the
// kernel's read-only AdminTenants/AdminUsers pages. True impersonation would only add attack surface.
//
// TO RE-ENABLE: uncomment the three lines below, uncomment <ImpersonationBanner/> in MainLayout.razor,
// make ImpersonationController public again, and restore the Impersonate UI on Customers.razor. The
// token-exchange service (LogtoImpersonationTokenService) also needs Impersonation:* + Authentication:Logto:* config.
// builder.Services.Configure<ImpersonationOptions>(
//     builder.Configuration.GetSection(ImpersonationOptions.SectionName));
// builder.Services.AddHttpClient<ILogtoImpersonationTokenService, LogtoImpersonationTokenService>();
// builder.Services.AddScoped<ImpersonationService>();

// -- Admin portal shell ---------------------------------------------------------
// The shell hosts one admin portal per deployed SaaS app. Per-app modules are
// runtime-loaded plugin dlls (e.g. SaaSFactory.Dating.Admin.dll) from the plugins
// folder - never compile-time references, so this public repo stays free of private
// identifiers. Every loaded assembly passes the kernel's security inspector or
// startup is aborted.
AdminPortalBuilder adminPortal = builder.Services.AddAdminPortalKernel(builder.Configuration);

// Read from ADMIN_PORTAL_PLUGINS_PATH env var (production/Docker) or AdminPortal:PluginsPath config key (local dev)
string? pluginsPath = builder.Configuration["ADMIN_PORTAL_PLUGINS_PATH"]
    ?? builder.Configuration["AdminPortal:PluginsPath"];
if (!string.IsNullOrWhiteSpace(pluginsPath))
{
    string resolvedPluginsPath = Path.GetFullPath(pluginsPath, builder.Environment.ContentRootPath);
    adminPortal.AddAdminPortalPlugins(resolvedPluginsPath);

    AdminPortalPluginLoadResult? loadResult = adminPortal.LastPluginLoadResult;
    if (loadResult is not null)
    {
        Console.WriteLine(loadResult.FolderFound
            ? $"[AdminPortal] Plugins folder: {resolvedPluginsPath} - loaded {loadResult.Modules.Count} module(s): {string.Join(", ", loadResult.Modules.Select(m => $"{m.Slug} ({m.RouterAssembly.GetName().Name} {m.RouterAssembly.GetName().Version})"))}"
            : $"[AdminPortal] Plugins folder not found: {resolvedPluginsPath} - no admin portal modules loaded");
        foreach (string skipped in loadResult.SkippedFiles)
        {
            Console.WriteLine($"[AdminPortal] Skipped non-assembly file: {skipped}");
        }
    }
}
else
{
    Console.WriteLine("[AdminPortal] ADMIN_PORTAL_PLUGINS_PATH or AdminPortal:PluginsPath not configured - no admin portal modules loaded");
}

// The audit log (dm_admin_audit) lives in DeploymentManager's own database; the table
// is created by DeploymentManager.ApiService's migrations. Required as soon as any
// admin portal module is loaded - audit is a security control, not best-effort.
string? deploymentManagerDb = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(deploymentManagerDb) && adminPortal.Registry.GetModules().Count > 0)
{
    throw new InvalidOperationException(
        "Admin portal modules are loaded but ConnectionStrings:DefaultConnection " +
        "(DeploymentManager database, hosts the dm_admin_audit log) is not configured.");
}
builder.Services.AddAdminPortalAuditStore(
    string.IsNullOrWhiteSpace(deploymentManagerDb) ? "Host=unconfigured" : deploymentManagerDb);

// Plugin modules may ship API controllers; they are hosted here behind the same
// role gate (enforced by the security inspector at load time).
IMvcBuilder adminPortalMvc = builder.Services.AddControllers();
if (adminPortal.LastPluginLoadResult is not null)
{
    foreach (System.Reflection.Assembly pluginAssembly in adminPortal.LastPluginLoadResult.Assemblies)
    {
        adminPortalMvc.AddApplicationPart(pluginAssembly);
    }
}
// ------------------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseForwardedHeaders();
app.UseRouting();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

// Defense in depth for the admin shell (mirrors AppBlueprint.Web): only whitelisted
// IPs reach admin routes in production. Configured via Security:AdminIpWhitelist.
if (!app.Environment.IsDevelopment())
{
    app.UseAdminIpWhitelist();
}

app.UseOutputCache();

app.MapAuthenticationEndpoints(builder.Configuration);

// The shared AppBlueprint authentication endpoints redirect post-login/post-signout users
// to /dashboard, /onboarding and /login - AppBlueprint.Web's page structure. DeploymentManager
// has no onboarding, its dashboard IS "/", and it has no separate login page, so bounce those
// shared targets to "/" (host-specific shim, like NullCurrentTenantService/NullApiKeyRepository).
// Without this, login lands on /onboarding -> 404.
app.MapGet("/dashboard", () => Results.LocalRedirect("/")).AllowAnonymous();
app.MapGet("/onboarding", () => Results.LocalRedirect("/")).AllowAnonymous();
app.MapGet("/login", () => Results.LocalRedirect("/")).AllowAnonymous();
app.MapGet("/ping", () => Results.Ok("ok")).AllowAnonymous();

// API controllers contributed by admin portal plugin modules (role-gated).
app.MapControllers();

// SECURITY (OWASP A01): no .AllowAnonymous() here - pages carry
// [Authorize(Roles = Roles.DeploymentManagerAdmin)] via Components/Pages/_Imports.razor,
// and AuthorizeRouteView in Routes.razor enforces it during interactive navigation.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

await app.RunAsync();

