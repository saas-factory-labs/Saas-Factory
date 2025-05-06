using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;

namespace AppBlueprint.Infrastructure.Repositories.Interfaces;

public interface ITeamRepository
{
    Task<IEnumerable<TeamEntity>> GetAllAsync();
    Task<TeamEntity> GetByIdAsync(int id);
    Task AddAsync(TeamEntity team);
    void Update(TeamEntity team);
    void Delete(int id);
}
