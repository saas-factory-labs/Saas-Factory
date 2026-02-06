using System.ComponentModel.DataAnnotations;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.AuditLog.Requests;

public class CreateAuditLogRequest
{
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

    public string Id { get; set; } = string.Empty;

    [DataClassification(GDPRType.IndirectlyIdentifiable)]
    public string Action { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    [Required] public string NewValue { get; set; } = string.Empty;
    [Required] public string OldValue { get; set; } = string.Empty;

    [Required] public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedAt { get; set; }

    // public int TenantId { get; set; }
    // change this to use a domain model instead
    // public TenantEntity Tenant { get; set; }
    // change this to use a domain model instead
    // // public int UserId { get; set; }
    // public UserEntity User { get; set; }
}
