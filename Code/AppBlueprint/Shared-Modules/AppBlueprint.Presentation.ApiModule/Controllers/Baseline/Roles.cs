using ApplicationRoles = AppBlueprint.Application.Constants.Roles;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Obsolete("Use AppBlueprint.Application.Constants.Roles instead. This compatibility type will be removed in a future release.")]
public static class Roles
{
    public const string DeploymentManagerAdmin = ApplicationRoles.DeploymentManagerAdmin;

    [Obsolete("Use AppBlueprint.Application.Constants.Roles.TenantAdmin for tenant-scoped administrators.")]
    public const string
        CustomerAdmin =
            "CustomerAdmin"; // Admin of a customer account that can manage users, settings, and subscriptions for their customer account and tenants

    [Obsolete("Use AppBlueprint.Application.Constants.Roles.User for standard tenant users.")]
    public const string
        CustomerUser = "CustomerUser"; // User that can only access their own resources and shared resources

    [Obsolete("No canonical replacement exists yet. Define the role in AppBlueprint.Application.Constants.Roles before using it.")]
    public const string
        RegisteredUser =
            "RegisteredUser"; // User that has registered an account but has not been assigned a role and is not yet a customer

    [Obsolete("No canonical replacement exists yet. Define the role in AppBlueprint.Application.Constants.Roles before using it.")]
    public const string
        GuestUser = "GuestUser"; // User that is a customer's guest user and has limited access to resources 

    [Obsolete("No canonical replacement exists yet. Define the role in AppBlueprint.Application.Constants.Roles before using it.")]
    public const string
        ExternalUser =
            "ExternalUser"; // User that is not a customer but has been invited to access resources such as a partner or contractor
}
