namespace AppBlueprint.AdminPortalKernel.Domain.Dtos;

/// <summary>
/// Diagnostics shown on a module's admin Debug tab so an operator can confirm which app,
/// which plugin dll and which database the portal is actually talking to. The connection
/// string is password-redacted; the live fields are read back from the database itself
/// (current_database/current_user/version) to expose the real target, not just config.
/// </summary>
public sealed record AdminPortalDebugInfo(
    string Slug,
    string DisplayName,
    string AssemblyName,
    string AssemblyVersion,
    string AssemblyLocation,
    string MaskedConnectionString,
    string? CurrentDatabase,
    string? CurrentUser,
    string? ServerVersion,
    IReadOnlyList<AdminPortalTableInfo> Tables,
    string? Error);

/// <summary>One table the admin portal reads, with its location and raw row count.</summary>
/// <param name="Name">Table name as written in SQL (e.g. <c>"Users"</c>).</param>
/// <param name="Database">Which database it lives in (app database vs DeploymentManager database).</param>
/// <param name="Exists">False when the table is missing or not readable on that connection.</param>
/// <param name="RowCount">Raw row count (app tables include soft-deleted rows so real data is visible).</param>
/// <param name="Note">Human-readable note about what the count represents.</param>
public sealed record AdminPortalTableInfo(
    string Name,
    string Database,
    bool Exists,
    int RowCount,
    string Note);
