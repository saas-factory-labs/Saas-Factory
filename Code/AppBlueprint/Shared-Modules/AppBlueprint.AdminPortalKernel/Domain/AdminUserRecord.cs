namespace AppBlueprint.AdminPortalKernel.Domain;

/// <summary>
/// Read model over the baseline "Users" table that every AppBlueprint-based app database
/// exposes. Maps the stable column subset only (see BaselineDbContextModelSnapshot.cs) so
/// the admin portal stays decoupled from each app's full schema. The only write performed
/// against this table is an ExecuteUpdate on <see cref="IsActive"/>.
/// </summary>
public sealed class AdminUserRecord
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? ExternalAuthId { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public DateTimeOffset LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
}
