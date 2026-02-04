using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.SharedModels;
using EmailVerificationEntity =
    AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.EmailVerification.EmailVerificationEntity;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

public sealed class UserEntity : BaseEntity, ITenantScoped
{
    private readonly List<EmailAddressEntity> _emailAddresses = new();
    private readonly List<AddressEntity> _addresses = new();
    private readonly List<EmailVerificationEntity> _emailVerifications = new();
    private readonly List<EmailInviteEntity> _referralInvitations = new();
    private readonly List<RoleEntity> _roles = new();
    private readonly List<ResourcePermissionEntity> _resourcePermissions = new();
    private readonly List<UserRoleEntity> _userRoles = new();

    public UserEntity()
    {
        Id = PrefixedUlid.Generate("user");
        IsActive = true;
        FirstName = string.Empty;
        LastName = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
        Profile = new ProfileEntity();
        TenantId = string.Empty;
    }

    [PIIRisk]
    public required string FirstName { get; set; }
    
    [PIIRisk]
    public required string LastName { get; set; }
    
    [PIIRisk]
    public required string UserName { get; set; }
    
    public bool IsActive { get; set; }

    [PIIRisk]
    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string? Email { get; set; }
    
    /// <summary>
    /// Generic metadata for the entity, including PII detection results.
    /// </summary>
    public EntityMetadata? Metadata { get; set; }

    /// <summary>
    /// External authentication provider user ID (e.g., Logto 'sub' claim).
    /// Used to correlate this user with their identity in the external auth system.
    /// </summary>
    public string? ExternalAuthId { get; set; }

    public DateTimeOffset LastLogin { get; set; } = DateTimeOffset.UtcNow;

    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public IReadOnlyCollection<EmailAddressEntity> EmailAddresses => _emailAddresses.AsReadOnly();
    public IReadOnlyCollection<AddressEntity> Addresses => _addresses.AsReadOnly();
    public IReadOnlyCollection<EmailVerificationEntity> EmailVerifications => _emailVerifications.AsReadOnly();
    public IReadOnlyCollection<EmailInviteEntity> ReferralInvitations => _referralInvitations.AsReadOnly();
    public IReadOnlyCollection<UserRoleEntity> UserRoles => _userRoles.AsReadOnly();

    public required ProfileEntity Profile { get; set; }

    public IReadOnlyCollection<RoleEntity> Roles => _roles.AsReadOnly();
    public IReadOnlyCollection<ResourcePermissionEntity> ResourcePermissions => _resourcePermissions.AsReadOnly();

    // Domain methods for controlled collection management
    public void AddEmailAddress(EmailAddressEntity emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);

        if (_emailAddresses.Any(ea => ea.Id == emailAddress.Id))
            return; // Email address already exists

        _emailAddresses.Add(emailAddress);
    }

    public void RemoveEmailAddress(EmailAddressEntity emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        _emailAddresses.Remove(emailAddress);
    }

    public void AddAddress(AddressEntity address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (_addresses.Any(a => a.Id == address.Id))
            return; // Address already exists

        _addresses.Add(address);
    }

    public void RemoveAddress(AddressEntity address)
    {
        ArgumentNullException.ThrowIfNull(address);
        _addresses.Remove(address);
    }

    public void AddEmailVerification(EmailVerificationEntity emailVerification)
    {
        ArgumentNullException.ThrowIfNull(emailVerification);

        if (_emailVerifications.Any(ev => ev.Id == emailVerification.Id))
            return; // Email verification already exists

        _emailVerifications.Add(emailVerification);
    }

    public void AddReferralInvitation(EmailInviteEntity invitation)
    {
        ArgumentNullException.ThrowIfNull(invitation);

        if (_referralInvitations.Any(ri => ri.Id == invitation.Id))
            return; // Invitation already exists

        _referralInvitations.Add(invitation);
    }

    public void AddRole(RoleEntity role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Any(r => r.Id == role.Id))
            return; // Role already exists

        _roles.Add(role);
    }

    public void RemoveRole(RoleEntity role)
    {
        ArgumentNullException.ThrowIfNull(role);
        _roles.Remove(role);
    }

    public void AddResourcePermission(ResourcePermissionEntity permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (_resourcePermissions.Any(rp => rp.Id == permission.Id))
            return; // Permission already exists

        _resourcePermissions.Add(permission);
    }

    public void RemoveResourcePermission(ResourcePermissionEntity permission)
    {
        ArgumentNullException.ThrowIfNull(permission);
        _resourcePermissions.Remove(permission);
    }

    public void AddUserRole(UserRoleEntity userRole)
    {
        ArgumentNullException.ThrowIfNull(userRole);

        if (_userRoles.Any(ur => ur.Id == userRole.Id))
            return; // User role already exists

        _userRoles.Add(userRole);
    }

    public void RemoveUserRole(UserRoleEntity userRole)
    {
        ArgumentNullException.ThrowIfNull(userRole);
        _userRoles.Remove(userRole);
    }
}
