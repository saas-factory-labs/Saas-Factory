using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.AdminPortalKernel.Domain;

/// <summary>
/// Read model over the baseline "Tenants" table shared by every AppBlueprint-based app.
/// Maps the stable column subset only; <see cref="TenantType"/> is stored as integer.
/// </summary>
public sealed class AdminTenantRecord
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TenantType TenantType { get; set; }
    public bool IsActive { get; set; }
    public bool IsSoftDeleted { get; set; }
    public string? Email { get; set; }
    public string? Country { get; set; }
    public string? VatNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
