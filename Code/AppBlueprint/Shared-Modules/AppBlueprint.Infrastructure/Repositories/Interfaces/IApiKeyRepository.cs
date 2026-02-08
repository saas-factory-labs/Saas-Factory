using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IApiKeyRepository
{
    Task<IEnumerable<ApiKeyEntity>> GetAllAsync();
    Task<ApiKeyEntity?> GetByIdAsync(string id);
    Task AddAsync(ApiKeyEntity apiKey);
    void Update(ApiKeyEntity apiKey);
    void Delete(string id);
}
