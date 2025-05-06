namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

public static class Roles
{
    public const string
        SaaSProviderAdmin =
            "SaaSProviderAdmin"; // Admin of the SaaS provider that owns the application and can manage all customers, users, and settings and subscriptions

    public const string
        CustomerAdmin =
            "CustomerAdmin"; // Admin of a customer account that can manage users, settings, and subscriptions for their customer account and tenants

    public const string
        CustomerUser = "CustomerUser"; // User that can only access their own resources and shared resources

    public const string
        RegisteredUser =
            "RegisteredUser"; // User that has registered an account but has not been assigned a role and is not yet a customer

    public const string
        GuestUser = "GuestUser"; // User that is a customer's guest user and has limited access to resources 

    public const string
        ExternalUser =
            "ExternalUser"; // User that is not a customer but has been invited to access resources such as a partner or contractor
}
