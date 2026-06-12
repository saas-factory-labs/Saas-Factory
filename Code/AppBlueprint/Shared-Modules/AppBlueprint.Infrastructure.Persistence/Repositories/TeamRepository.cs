using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B.Entities.Team.Team;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using B2BDbContext = AppBlueprint.Infrastructure.Persistence.DatabaseContexts.B2B.B2BDbContext;

namespace AppBlueprint.Infrastructure.Persistence.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly B2BDbContext _context;

    public TeamRepository(B2BDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TeamEntity>> GetAllAsync()
    {
        return await _context.Teams.ToListAsync();
    }

    public async Task<TeamEntity?> GetByIdAsync(string id)
    {
        return await _context.Teams.FindAsync(id);
    }

    public async Task AddAsync(TeamEntity team)
    {
        await _context.Teams.AddAsync(team);
    }

    public void Update(TeamEntity team)
    {
        _context.Teams.Update(team);
    }
    public void Delete(string id)
    {
        TeamEntity? team = _context.Teams.Find(id);
        if (team is not null) _context.Teams.Remove(team);
    }
}
