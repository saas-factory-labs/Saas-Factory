using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;

    public TeamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TeamEntity>> GetAllAsync()
    {
        return await _context.Set<TeamEntity>().ToListAsync();
    }

    public async Task<TeamEntity> GetByIdAsync(string id)
    {
        return await _context.Set<TeamEntity>().FindAsync(id);
    }

    public async Task AddAsync(TeamEntity team)
    {
        await _context.Set<TeamEntity>().AddAsync(team);
    }

    public void Update(TeamEntity team)
    {
        _context.Set<TeamEntity>().Update(team);
    }    public void Delete(string id)
    {
        TeamEntity? team = _context.Set<TeamEntity>().Find(id);
        if (team is not null) _context.Set<TeamEntity>().Remove(team);
    }
}
