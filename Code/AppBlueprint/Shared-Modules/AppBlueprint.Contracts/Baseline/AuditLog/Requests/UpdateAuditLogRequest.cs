using System.ComponentModel.DataAnnotations;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.User.Requests;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.AuditLog.Requests;

public class UpdateAuditLogRequest
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
    public string? Action { get; set; }

    public string? Category { get; set; }

    [Required] public string? NewValue { get; set; }
    [Required] public string? OldValue { get; set; }

    [Required] public string? ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }

    // public int TenantId { get; set; }
    public UpdateTenantRequest? Tenant { get; set; }

    // public int UserId { get; set; }
    public UpdateUserRequest? User { get; set; }
}
