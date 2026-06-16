using AppBlueprint.AdminPortalKernel.Modules;

namespace SaaSFactory.Sample.Admin;

/// <summary>
/// Admin portal module for the sample app. The DeploymentManager shell discovers this
/// class when the dll is dropped in the plugins folder; the generic dashboard, users,
/// tenants and audit pages work immediately once
/// AdminPortal:Modules:sample:ConnectionString points at the app's database.
/// </summary>
public sealed class SampleAdminModule : IAdminPortalModule
{
    public string Slug => "sample";

    public string DisplayName => "SaaS Factory Core";

    /// <summary>
    /// Local dev URL of AppBlueprint.Web, used when the DeploymentManager shell runs in Development.
    /// The sample app has no public production site, so <see cref="IAdminPortalModule.SiteUrl"/> stays
    /// null and the "Visit site" link is hidden when deployed.
    /// </summary>
    public string? LocalSiteUrl => "https://localhost:8083";

    public IReadOnlyList<AdminPortalNavItem> ExtraNavItems { get; } =
    [
        new AdminPortalNavItem("Custom page", "/apps/sample/admin/custom", Icon: string.Empty)
    ];
}
