using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;

/// <summary>
/// Represents a tenant in the multi-tenant system.
/// Supports both B2C (Personal) and B2B (Organization) tenancy models.
/// </summary>
/// <remarks>
/// Architecture follows Microsoft's multi-tenancy best practices:
/// https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models
/// 
/// B2C (Personal Tenant):
/// - Auto-created 1:1 with user registration
/// - Simple personal account (dating app profile, individual rental listing)
/// - Optional B2B fields are null
/// 
/// B2B (Organization Tenant):
/// - Explicitly created for business entities
/// - Multiple users via teams and roles
/// - Full B2B features (VAT, contact persons, organizational hierarchy)
/// </remarks>
public sealed class TenantEntity : BaseEntity
{
    private readonly List<ContactPersonEntity> _contactPersons = new();
    private readonly List<UserEntity> _users = new();

    public TenantEntity()
    {
        Id = PrefixedUlid.Generate("tenant");
        Name = string.Empty;
        TenantType = TenantType.Personal; // Default to B2C
    }

    /// <summary>
    /// Type of tenant (Personal for B2C, Organization for B2B).
    /// </summary>
    public TenantType TenantType { get; set; }

    /// <summary>
    /// Tenant name. For Personal: user's full name. For Organization: company name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Optional description. Typically used for Organization tenants.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the tenant is active and can access the system.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indicates if this is the user's primary tenant (for B2C multi-tenant scenarios).
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Contact email for the tenant.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Contact phone for the tenant.
    /// </summary>
    public string? Phone { get; set; }

    // ========================================
    // B2B-Specific Fields (Nullable for B2C)
    // ========================================

    /// <summary>
    /// VAT/Tax number. Only for Organization tenants (B2B).
    /// </summary>
    public string? VatNumber { get; set; }

    /// <summary>
    /// Country code. Only for Organization tenants (B2B).
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Customer billing information. Can be shared between B2C and B2B.
    /// </summary>
    public CustomerEntity? Customer { get; set; }

    // ========================================
    // Relationships
    // ========================================

    /// <summary>
    /// Contact persons for this tenant. Typically only for Organization tenants (B2B).
    /// </summary>
    public IReadOnlyCollection<ContactPersonEntity> ContactPersons => _contactPersons.AsReadOnly();

    /// <summary>
    /// Users belonging to this tenant.
    /// B2C Personal: Usually 1 user (or small family group).
    /// B2B Organization: Multiple users with team-based access.
    /// </summary>
    public IReadOnlyCollection<UserEntity> Users => _users.AsReadOnly();

    // ========================================
    // Domain Methods
    // ========================================

    /// <summary>
    /// Adds a contact person to this tenant (B2B feature).
    /// </summary>
    public void AddContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);

        if (_contactPersons.Any(cp => cp.Id == contactPerson.Id))
            return; // Contact person already exists

        _contactPersons.Add(contactPerson);
    }

    /// <summary>
    /// Removes a contact person from this tenant (B2B feature).
    /// </summary>
    public void RemoveContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);
        _contactPersons.Remove(contactPerson);
    }

    /// <summary>
    /// Adds a user to this tenant.
    /// </summary>
    public void AddUser(UserEntity user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (_users.Any(u => u.Id == user.Id))
            return; // User already exists

        _users.Add(user);
        user.TenantId = Id;
    }

    /// <summary>
    /// Removes a user from this tenant.
    /// </summary>
    public void RemoveUser(UserEntity user)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users.Remove(user);
    }

    /// <summary>
    /// Creates a Personal tenant for B2C scenarios (individual user or family).
    /// </summary>
    public static TenantEntity CreatePersonalTenant(string name, string? email = null)
    {
        return new TenantEntity
        {
            TenantType = TenantType.Personal,
            Name = name,
            Email = email,
            IsActive = true,
            IsPrimary = true
        };
    }

    /// <summary>
    /// Creates an Organization tenant for B2B scenarios (business entity).
    /// </summary>
    public static TenantEntity CreateOrganizationTenant(
        string organizationName,
        string email,
        string? vatNumber = null,
        string? country = null)
    {
        return new TenantEntity
        {
            TenantType = TenantType.Organization,
            Name = organizationName,
            Email = email,
            VatNumber = vatNumber,
            Country = country,
            IsActive = true,
            IsPrimary = false
        };
    }
}
