namespace AppBlueprint.AdminPortalKernel.Infrastructure;

/// <summary>Creates EF contexts for a specific admin portal module's app database.</summary>
public interface IAdminPortalDbContextFactory
{
    /// <summary>
    /// Creates a context connected to the app database of the module with the given slug.
    /// Throws when no connection string is configured for the slug.
    /// The caller owns disposal.
    /// </summary>
    AdminPortalAppDbContext CreateForModule(string slug);
}
