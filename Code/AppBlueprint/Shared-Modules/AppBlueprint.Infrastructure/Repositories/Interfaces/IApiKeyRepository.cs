using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<IEnumerable<ApiKeyEntity>> GetAllAsync();
    Task<ApiKeyEntity?> GetByIdAsync(string id);
    /// <summary>Looks up an API key by the SHA-256 hex hash of its raw secret value.</summary>
    Task<ApiKeyEntity?> GetByKeyHashAsync(string keyHash);
    Task AddAsync(ApiKeyEntity apiKey);
    void Update(ApiKeyEntity apiKey);
    void Delete(string id);
}
