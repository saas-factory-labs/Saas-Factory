using System.ComponentModel.DataAnnotations;
using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;

public class AuditLogEntity
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

    public int Id { get; set; }

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
    //public int EntityId { get; set; }

    [DataClassification(GDPRType.Sensitive)]
    public required string Action { get; set; } // GDPR

    public string? Category { get; set; }

    [Required] public required string NewValue { get; set; }
    [Required] public required string OldValue { get; set; }

    [Required] public required UserEntity ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }

    public int TenantId { get; set; }
    public required TenantEntity Tenant { get; set; }

    public int UserId { get; set; }
    public required UserEntity User { get; set; }
}
