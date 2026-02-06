using System.ComponentModel.DataAnnotations;
using AppBlueprint.Contracts.B2B.Contracts.Tenant.Responses;
using AppBlueprint.Contracts.Baseline.User.Responses;

namespace AppBlueprint.Contracts.Baseline.AuditLog.Responses;

public class AuditLogResponse
{
    public string Id { get; init; } = string.Empty;

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
    public required string Action { get; init; }
    public required string Category { get; init; }

    [Required] public required string NewValue { get; init; }
    [Required] public required string OldValue { get; init; }

    [Required] public required string ModifiedBy { get; init; }
    public required DateTime ModifiedAt { get; init; }

    // public int TenantId { get; set; }
    public required TenantResponse Tenant { get; init; }

    // public int UserId { get; set; }
    public required UserResponse User { get; init; }
}
