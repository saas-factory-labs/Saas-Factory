using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface IOrganizationRepository
{
    Task<IEnumerable<OrganizationEntity>> GetAllAsync();
    Task<OrganizationEntity> GetByIdAsync(int id);
    Task AddAsync(OrganizationEntity organization);
    void Update(OrganizationEntity organization);
    void Delete(int id);
}
