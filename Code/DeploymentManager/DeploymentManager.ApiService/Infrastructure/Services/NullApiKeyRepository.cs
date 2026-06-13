using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B.Entities;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;

namespace DeploymentManager.ApiService.Infrastructure.Services;

/// <summary>
/// No-op API key repository for the DeploymentManager API.
/// <para>
/// AppBlueprint's shared <c>AddJwtAuthentication</c> always registers the
/// <c>ApiKeyAuthenticationHandler</c> (x-api-key) scheme, whose constructor needs an
/// <see cref="IApiKeyRepository"/>. DeploymentManager authenticates exclusively via
/// Logto JWT bearer + the <c>DeploymentManagerAdmin</c> role and deliberately does not
/// support API keys, so this implementation never resolves a key — the ApiKey scheme is
/// constructible but always returns "no result". This avoids pulling the full
/// AppBlueprint persistence stack (B2BDbContext etc.) into this internal admin API.
/// </para>
/// </summary>
public sealed class NullApiKeyRepository : IApiKeyRepository
{
    public Task<IEnumerable<ApiKeyEntity>> GetAllAsync() =>
        Task.FromResult(Enumerable.Empty<ApiKeyEntity>());

    public Task<ApiKeyEntity?> GetByIdAsync(string id) => Task.FromResult<ApiKeyEntity?>(null);

    public Task<ApiKeyEntity?> GetByKeyHashAsync(string keyHash) => Task.FromResult<ApiKeyEntity?>(null);

    public Task AddAsync(ApiKeyEntity apiKey) =>
        throw new NotSupportedException("The DeploymentManager API does not support API keys.");

    public void Update(ApiKeyEntity apiKey) =>
        throw new NotSupportedException("The DeploymentManager API does not support API keys.");

    public void Delete(string id) =>
        throw new NotSupportedException("The DeploymentManager API does not support API keys.");

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(0);
}
