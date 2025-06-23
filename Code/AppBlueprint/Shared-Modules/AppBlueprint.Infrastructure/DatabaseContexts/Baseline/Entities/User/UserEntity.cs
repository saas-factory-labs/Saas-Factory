using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Addressing;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Authorization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailAddress;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Email.EmailInvite;
using AppBlueprint.SharedKernel;
using EmailVerificationEntity =
    AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.EmailVerification.EmailVerificationEntity;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

public class UserEntity : BaseEntity, ITenantScoped
{    public UserEntity()
    {
        Id = PrefixedUlid.Generate("user");
        EmailAddresses = new List<EmailAddressEntity>();
        Addresses = new List<AddressEntity>();
        EmailVerifications = new List<EmailVerificationEntity>();
        ReferralInvitations = new List<EmailInviteEntity>();
        Roles = new List<RoleEntity>();
        ResourcePermissions = new List<ResourcePermissionEntity>();
        UserRoles = new List<UserRoleEntity>();

        IsActive = true;

        FirstName = string.Empty;
        LastName = string.Empty;
        UserName = string.Empty;
        Email = string.Empty;
        Profile = new ProfileEntity();
        TenantId = string.Empty;
    }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string UserName { get; set; }
    public bool IsActive { get; set; }

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string? Email { get; set; }

    public DateTimeOffset LastLogin { get; set; } = DateTimeOffset.Now;    // ITenantScoped implementation
    public string TenantId { get; set; }
    public TenantEntity? Tenant { get; set; }

    public List<EmailAddressEntity> EmailAddresses { get; init; }
    public List<AddressEntity> Addresses { get; init; }
    public List<EmailVerificationEntity> EmailVerifications { get; init; }
    public List<EmailInviteEntity> ReferralInvitations { get; init; }
    public List<UserRoleEntity> UserRoles { get; init; }

    public required ProfileEntity Profile { get; set; }

    public List<RoleEntity> Roles { get; init; }
    public List<ResourcePermissionEntity> ResourcePermissions { get; init; }
}
