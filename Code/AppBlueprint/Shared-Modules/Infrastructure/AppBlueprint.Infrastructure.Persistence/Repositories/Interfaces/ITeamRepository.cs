using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B.Entities.Team.Team;

namespace AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<IEnumerable<TeamEntity>> GetAllAsync();
    Task<TeamEntity?> GetByIdAsync(string id);
    Task AddAsync(TeamEntity team);
    void Update(TeamEntity team);
    void Delete(string id);
}
