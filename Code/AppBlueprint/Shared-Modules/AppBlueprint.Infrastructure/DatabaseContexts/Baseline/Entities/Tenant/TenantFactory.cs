using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;

/// <summary>
/// Factory for creating tenants following Microsoft's multi-tenancy patterns.
/// Supports both B2C (Personal) and B2B (Organization) scenarios.
/// </summary>
/// <remarks>
/// Based on Microsoft's guidance:
/// https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models
/// 
/// B2C (Personal): Individual users, families, small groups
/// B2B (Organization): Companies with multiple users, teams, enterprise features
/// </remarks>
public static class TenantFactory
{
    /// <summary>
    /// Creates a Personal tenant for B2C scenarios.
    /// Automatically created 1:1 with user registration.
    /// </summary>
    /// <param name="user">The user for whom to create a personal tenant.</param>
    /// <returns>A new Personal tenant instance.</returns>
    /// <example>
    /// Use cases:
    /// - Dating app: Each user gets personal profile/tenant
    /// - Property rental: Individual landlord or renter account
    /// - Personal productivity app: Individual user workspace
    /// </example>
    public static TenantEntity CreatePersonalTenant(UserEntity user)
    {
        ArgumentNullException.ThrowIfNull(user);

        string tenantName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(tenantName))
        {
            tenantName = user.UserName;
        }

        return TenantEntity.CreatePersonalTenant(
            name: tenantName,
            email: user.Email
        );
    }

    /// <summary>
    /// Creates a Personal tenant for a family or small group (B2C).
    /// Used when multiple users share a single account (e.g., family plan).
    /// </summary>
    /// <param name="familyName">Name of the family or group.</param>
    /// <param name="primaryUserEmail">Email of the primary user.</param>
    /// <returns>A new Personal tenant instance for a group.</returns>
    /// <example>
    /// Use cases:
    /// - Music streaming family plan
    /// - Shared photo album
    /// - Family budget tracker
    /// </example>
    public static TenantEntity CreateFamilyTenant(string familyName, string primaryUserEmail)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(primaryUserEmail);

        return TenantEntity.CreatePersonalTenant(
            name: $"{familyName} Family",
            email: primaryUserEmail
        );
    }

    /// <summary>
    /// Creates an Organization tenant for B2B scenarios.
    /// Explicitly created for business entities with multiple users and teams.
    /// </summary>
    /// <param name="organizationName">Name of the organization/company.</param>
    /// <param name="organizationEmail">Official organization email.</param>
    /// <param name="vatNumber">Optional VAT/Tax number for invoicing.</param>
    /// <param name="country">Optional country code.</param>
    /// <returns>A new Organization tenant instance.</returns>
    /// <example>
    /// Use cases:
    /// - CRM: Each client company is an organization tenant
    /// - Project management SaaS: Each company workspace
    /// - Enterprise collaboration tool: Company-wide platform
    /// </example>
    public static TenantEntity CreateOrganizationTenant(
        string organizationName,
        string organizationEmail,
        string? vatNumber = null,
        string? country = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationName);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationEmail);

        return TenantEntity.CreateOrganizationTenant(
            organizationName: organizationName,
            email: organizationEmail,
            vatNumber: vatNumber,
            country: country
        );
    }

    /// <summary>
    /// Determines the appropriate tenant type based on context.
    /// Helper method for automatic tenant creation.
    /// </summary>
    /// <param name="isBusinessRegistration">Whether this is a business registration.</param>
    /// <param name="hasVatNumber">Whether a VAT number was provided.</param>
    /// <returns>The recommended tenant type.</returns>
    public static TenantType DetermineTenantType(bool isBusinessRegistration, bool hasVatNumber)
    {
        return isBusinessRegistration || hasVatNumber
            ? TenantType.Organization
            : TenantType.Personal;
    }

    /// <summary>
    /// Validates if a user can create a new organization tenant.
    /// Business rules: User must be active and not exceed org creation limits.
    /// </summary>
    /// <param name="user">The user attempting to create an organization.</param>
    /// <param name="existingOrganizationCount">Number of organizations user already owns.</param>
    /// <param name="maxOrganizationsPerUser">Maximum organizations allowed per user.</param>
    /// <returns>True if user can create organization, false otherwise.</returns>
    public static bool CanUserCreateOrganization(
        UserEntity user,
        int existingOrganizationCount,
        int maxOrganizationsPerUser = 5)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Business rules for organization creation
        bool isActiveUser = user.IsActive;
        bool hasNotExceededLimit = existingOrganizationCount < maxOrganizationsPerUser;

        return isActiveUser && hasNotExceededLimit;
    }
}
