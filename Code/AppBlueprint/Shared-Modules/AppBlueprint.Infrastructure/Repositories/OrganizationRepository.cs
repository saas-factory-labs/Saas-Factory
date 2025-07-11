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
        return await _context.Organizations.ToListAsync();
    }
    public async Task<OrganizationEntity?> GetByIdAsync(string id)
    {
        return await _context.Organizations.FindAsync(id);
    }

    public async Task AddAsync(OrganizationEntity organization)
    {
        await _context.Organizations.AddAsync(organization);
    }

    public void Update(OrganizationEntity organization)
    {
        _context.Organizations.Update(organization);
    }

    public void Delete(string id)
    {
        OrganizationEntity? organization = _context.Organizations.Find(id);
        if (organization is not null) _context.Organizations.Remove(organization);
    }
}
