using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IApiKeyRepository
{
    public Task<IEnumerable<ApiKeyEntity>> GetAllAsync();
    public Task<ApiKeyEntity> GetByIdAsync(int id);
    public Task AddAsync(ApiKeyEntity apiKey);
    public void Update(ApiKeyEntity apiKey);
    public void Delete(int id);
}
