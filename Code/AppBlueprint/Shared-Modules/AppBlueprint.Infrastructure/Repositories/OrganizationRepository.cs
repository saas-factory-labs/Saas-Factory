using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Organization;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly ApplicationDbContext _context;

    public OrganizationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrganizationEntity>> GetAllAsync()
    {
        return await _context.Set<OrganizationEntity>().ToListAsync();
    }    public async Task<OrganizationEntity> GetByIdAsync(string id)
    {
        return await _context.Set<OrganizationEntity>().FindAsync(id);
    }

    public async Task AddAsync(OrganizationEntity organization)
    {
        await _context.Set<OrganizationEntity>().AddAsync(organization);
    }

    public void Update(OrganizationEntity organization)
    {
        _context.Set<OrganizationEntity>().Update(organization);
    }

    public void Delete(int id)
    {
        OrganizationEntity? organization = _context.Set<OrganizationEntity>().Find(id);
        if (organization is not null) _context.Set<OrganizationEntity>().Remove(organization);
    }
}
