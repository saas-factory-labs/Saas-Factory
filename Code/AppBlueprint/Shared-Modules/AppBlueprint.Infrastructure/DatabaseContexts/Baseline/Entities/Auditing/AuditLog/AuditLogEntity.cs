using System.ComponentModel.DataAnnotations;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.Profile;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;
using AppBlueprint.SharedKernel.SharedModels;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Auditing.AuditLog;

public class AuditLogEntity : BaseEntity, ITenantScoped
{
    public AuditLogEntity()
    {
        User = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
            UserName = "UserName",
            Email = "Email",
            Profile = new ProfileEntity()
        };
        Tenant = new TenantEntity();
    }

    // create new user, update user, delete user, etc.
    // create new customer, update customer, delete customer, etc.
    // create new tenant, update tenant, delete tenant, etc.
    // create new role, update role, delete role, etc.
    // create new permission, update permission, delete permission, etc.
    // create new user role, update user role, delete user role, etc.
    // create new address, update address, delete address, etc.
    // create new email, update email, delete email, etc.
    // create new phone number, update phone number, delete phone number, etc.
    // create new contact person, update contact person, delete contact person, etc.    

    //public string EntityName { get; set; }
    //public string EntityId { get; set; }

    [DataClassification(GDPRType.SensitiveMiscellaneous)]
    public required string Action { get; set; } // GDPR

    public string? Category { get; set; }

    [Required]
    [PIIRisk]
    public required string NewValue { get; set; }

    [Required]
    [PIIRisk]
    public required string OldValue { get; set; }

    /// <summary>
    /// Generic metadata for the entity, including PII detection results.
    /// </summary>
    public EntityMetadata? Metadata { get; set; }

    [Required] public required UserEntity ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }

    public string TenantId { get; set; } = string.Empty;
    public required TenantEntity Tenant { get; set; }

    public string UserId { get; set; } = string.Empty;
    public required UserEntity User { get; set; }
}
