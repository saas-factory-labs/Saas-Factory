using AppBlueprint.Application.Services;
using AppBlueprint.Infrastructure.Repositories.Interfaces;

namespace AppBlueprint.Infrastructure.Services;

/// <summary>
/// Implementation of ICurrentTenantService that retrieves tenant information from the database.
/// Extracts tenant ID from JWT claims and fetches tenant details.
/// </summary>
public sealed class CurrentTenantService : ICurrentTenantService
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ITenantRepository _tenantRepository;

    public CurrentTenantService(
        ITenantContextAccessor tenantContextAccessor,
        ITenantRepository tenantRepository)
    {
        ArgumentNullException.ThrowIfNull(tenantContextAccessor);
        ArgumentNullException.ThrowIfNull(tenantRepository);

        _tenantContextAccessor = tenantContextAccessor;
        _tenantRepository = tenantRepository;
    }

    /// <summary>
    /// Gets the current tenant's ID from JWT "tenant_id" claim.
    /// Returns null if no authenticated user exists or tenant claim is missing.
    /// </summary>
    public string? TenantId => _tenantContextAccessor.TenantId;

    /// <summary>
    /// Gets the current tenant's name from the database.
    /// Returns null if no tenant context is available or tenant is not found.
    /// </summary>
    public async Task<string?> GetTenantNameAsync()
    {
        string? tenantId = _tenantContextAccessor.TenantId;

        if (string.IsNullOrEmpty(tenantId))
            return null;

        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        return tenant?.Name;
    }

    /// <summary>
    /// Gets the current tenant's type from the database.
    /// Returns null if no tenant context is available or tenant is not found.
    /// </summary>
    public async Task<SharedKernel.Enums.TenantType?> GetTenantTypeAsync()
    {
        string? tenantId = _tenantContextAccessor.TenantId;

        if (string.IsNullOrEmpty(tenantId))
            return null;

        var tenant = await _tenantRepository.GetByIdAsync(tenantId);

        return tenant?.TenantType;
    }
}
