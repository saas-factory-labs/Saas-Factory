using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Application.Services;

/// <summary>
/// Service for accessing current tenant information from authenticated user context.
/// Used by UI components to display tenant information.
/// </summary>
public interface ICurrentTenantService
{
    /// <summary>
    /// Gets the current tenant's name (company name for B2B, user's full name for B2C).
    /// Returns null if no authenticated user or tenant information is not available.
    /// </summary>
    Task<string?> GetTenantNameAsync();

    /// <summary>
    /// Gets the current tenant's type (Personal for B2C, Organization for B2B).
    /// Returns null if no authenticated user or tenant information is not available.
    /// </summary>
    Task<TenantType?> GetTenantTypeAsync();

    /// <summary>
    /// Gets the current tenant's ID from JWT claims.
    /// Returns null if no authenticated user exists.
    /// </summary>
    string? TenantId { get; }
}
