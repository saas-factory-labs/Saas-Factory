namespace AppBlueprint.Application.Constants;

public static class Roles
{
    /// <summary>
    /// Deployment Manager administrator - has access to all tenants across all deployed B2C/B2B SaaS apps.
    /// This role is used by the DeploymentManager project for cross-tenant operations and system configuration.
    /// Use this role for platform-level administrative operations.
    /// </summary>
    public const string DeploymentManagerAdmin = "DeploymentManagerAdmin";
    
    /// <summary>
    /// Admin of a specific tenant/customer.
    /// Has full access within their own tenant only.
    /// </summary>
    public const string TenantAdmin = "TenantAdmin";
    
    /// <summary>
    /// Regular user within a tenant.
    /// Has standard user permissions.
    /// </summary>
    public const string User = "User";
}
