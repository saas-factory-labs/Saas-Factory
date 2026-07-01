using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.AdminPortalKernel.Modules;

/// <summary>
/// Contract implemented by every per-app admin portal module (plugin).
/// A module is shipped as a Razor Class Library dll (e.g. SaaSFactory.Dating.Admin.dll),
/// loaded by the DeploymentManager shell either at runtime from the plugins folder or
/// registered at compile time, and rendered inside the shell's authenticated UI.
/// </summary>
public interface IAdminPortalModule
{
    /// <summary>
    /// URL-safe identifier for the app (lowercase letters, digits, hyphens).
    /// All module routes live under <c>/apps/{Slug}/admin</c> and the app database
    /// connection string is resolved from <c>AdminPortal:Modules:{Slug}:ConnectionString</c>.
    /// </summary>
    string Slug { get; }

    /// <summary>Human-readable name shown in the shell navigation (e.g. "Dating App").</summary>
    string DisplayName { get; }

    /// <summary>
    /// Optional inline SVG markup for the shell navigation entry (Cruip/Tailwind style,
    /// matching the UiKit sidebar icons). Null renders the shell's default icon.
    /// </summary>
    string? Icon => null;

    /// <summary>
    /// Public production URL of the running SaaS app (e.g. <c>https://boligmagneten.dk</c>),
    /// surfaced as a "Visit site" link in the shell's per-app header. Null hides the link
    /// when the shell runs in a non-development environment.
    /// </summary>
    Uri? SiteUrl => null;

    /// <summary>
    /// Local development URL of the SaaS app (e.g. <c>https://localhost:7247</c>), used instead of
    /// <see cref="SiteUrl"/> when the DeploymentManager shell itself runs in the Development
    /// environment. Null falls back to <see cref="SiteUrl"/>.
    /// </summary>
    Uri? LocalSiteUrl => null;

    /// <summary>
    /// Optional brand tokens (colors, button radius) applied to this app's pages in the shell so
    /// each SaaS app renders in its own brand rather than the shell's default violet. Null uses the
    /// shell defaults. The brand definition lives with the module in the app's own repo.
    /// </summary>
    AdminPortalBranding? Branding => null;

    /// <summary>
    /// Resolves the "Visit site" link for the current host environment: the local dev URL when the
    /// shell runs in Development, otherwise the public production URL. Returns null when no suitable
    /// URL is configured, in which case the shell hides the link.
    /// </summary>
    /// <param name="isDevelopment">Whether the DeploymentManager shell runs in the Development environment.</param>
    Uri? ResolveSiteUrl(bool isDevelopment)
    {
        if (isDevelopment && LocalSiteUrl is not null)
        {
            return LocalSiteUrl;
        }

        return SiteUrl;
    }

    /// <summary>
    /// Assembly containing the module's routable Blazor components.
    /// Added to the shell Router's AdditionalAssemblies.
    /// </summary>
    Assembly RouterAssembly => GetType().Assembly;

    /// <summary>Optional extra navigation items beyond the generic dashboard/users/tenants/audit pages.</summary>
    IReadOnlyList<AdminPortalNavItem> ExtraNavItems => Array.Empty<AdminPortalNavItem>();

    /// <summary>Optional hook for registering module-specific services in the host container.</summary>
    void ConfigureServices(IServiceCollection services)
    {
    }

    /// <summary>
    /// Live SaaS metrics (MRR, churn, signups, paying customers) for the shell's per-app
    /// "SaaS Metrics" page. Defaults to <see cref="SaasAppMetrics.Empty"/> so a module only
    /// implements it once it has a metrics source wired up.
    /// </summary>
    Task<SaasAppMetrics> GetLiveMetricsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(SaasAppMetrics.Empty);

    /// <summary>
    /// Optional JSON describing the app's structural dependency graph (nodes + edges), rendered on
    /// the shell's per-app Dependency Map page. Null falls back to a generic graph derived from the
    /// app's baseline schema.
    /// </summary>
    string? DependencyStructureJson => null;
}
