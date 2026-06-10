using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Authentication;
using AppBlueprint.Infrastructure.Extensions;
using MudBlazor.Services;
using AppBlueprint.ServiceDefaults;
using AppBlueprint.UiKit;
using AppBlueprint.UiKit.Services;
using DeploymentManager.Web.Components;
using DeploymentManager.Web.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

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

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

if (builder.Environment.IsDevelopment())
{
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        // Unspecified = don't enforce a minimum — lets SameSite=None on OIDC correlation/nonce
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
builder.Services.AddMudServices();
builder.Services.AddUiKit();
builder.Services.AddHttpContextAccessor();
builder.Services.AddWebAuthentication(builder.Configuration, builder.Environment);

// DeploymentManager-specific services
builder.Services.AddScoped<IMenuConfigurationService, MenuConfigurationService>();

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
app.UseOutputCache();

app.MapAuthenticationEndpoints(builder.Configuration);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.MapDefaultEndpoints();

await app.RunAsync();
