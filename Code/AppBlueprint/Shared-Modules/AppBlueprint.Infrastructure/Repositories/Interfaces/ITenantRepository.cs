using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Repository interface for tenant entities.
/// Uses async methods for thread-safe Blazor Server compatibility.
/// </summary>
public interface ITenantRepository
{
    Task<IEnumerable<TenantEntity>> GetAllAsync();
    Task<TenantEntity?> GetByIdAsync(string id);
    Task AddAsync(TenantEntity tenant);
    Task UpdateAsync(TenantEntity tenant);
    Task DeleteAsync(string id);
    
    /// <summary>
    /// Synchronous update - not recommended for Blazor Server.
    /// Use UpdateAsync instead.
    /// </summary>
    [Obsolete("Use UpdateAsync for thread-safe operations in Blazor Server")]
    void Update(TenantEntity tenant);
    
    /// <summary>
    /// Synchronous delete - not recommended for Blazor Server.
    /// Use DeleteAsync instead.
    /// </summary>
    [Obsolete("Use DeleteAsync for thread-safe operations in Blazor Server")]
    void Delete(string id);
}

