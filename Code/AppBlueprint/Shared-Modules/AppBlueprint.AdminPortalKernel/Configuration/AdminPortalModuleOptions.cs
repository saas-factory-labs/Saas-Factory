namespace AppBlueprint.AdminPortalKernel.Configuration;

/// <summary>Settings for a single admin portal module.</summary>
public sealed class AdminPortalModuleOptions
{
    /// <summary>
    /// Connection string for the target app's own PostgreSQL database (Neon/Railway).
    /// Use a dedicated per-app DB user with SELECT on "Users"/"Tenants" and UPDATE on "Users".
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
