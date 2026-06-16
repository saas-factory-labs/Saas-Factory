namespace AppBlueprint.AdminPortalKernel.Configuration;

/// <summary>
/// Host configuration for the admin portal shell, bound from the <c>AdminPortal</c> section.
/// Railway environment variable format: <c>AdminPortal__Modules__{slug}__ConnectionString</c>.
/// </summary>
public sealed class AdminPortalOptions
{
    public const string SectionName = "AdminPortal";

    /// <summary>
    /// Folder scanned for plugin dlls at startup. Relative paths are resolved against the
    /// host content root. Empty/missing means no runtime plugins are loaded.
    /// </summary>
    public string? PluginsPath { get; set; }

    /// <summary>Per-module settings keyed by module slug.</summary>
    public Dictionary<string, AdminPortalModuleOptions> Modules { get; } = new(StringComparer.OrdinalIgnoreCase);
}
