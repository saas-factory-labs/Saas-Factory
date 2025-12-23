namespace AppBlueprint.SharedKernel.Enums;

/// <summary>
/// Defines the type of tenant in the multi-tenant system.
/// Supports both B2C (Business-to-Consumer) and B2B (Business-to-Business) scenarios.
/// </summary>
/// <remarks>
/// Based on Microsoft's multi-tenancy architecture patterns:
/// https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models
/// 
/// - Personal: B2C scenarios (individual users, families, small groups)
/// - Organization: B2B scenarios (companies with multiple users, teams, enterprise features)
/// </remarks>
public enum TenantType
{
    /// <summary>
    /// Personal tenant for B2C scenarios.
    /// Represents individual users or small family groups.
    /// Typically 1:1 or 1:few user-to-tenant ratio.
    /// Auto-created on user registration.
    /// Examples: Dating app profiles, individual rental listings, personal accounts.
    /// </summary>
    Personal = 0,

    /// <summary>
    /// Organizational tenant for B2B scenarios.
    /// Represents businesses with multiple users, teams, and enterprise features.
    /// Supports multi-user collaboration, roles, and organizational hierarchy.
    /// Explicitly created for business entities.
    /// Examples: CRM organizations, enterprise SaaS customers, business teams.
    /// </summary>
    Organization = 1
}
