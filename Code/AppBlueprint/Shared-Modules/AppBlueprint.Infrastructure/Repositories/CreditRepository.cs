using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Repositories;

public class CreditRepository : ICreditRepository
{
    private readonly ApplicationDbContext _context;

    public CreditRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CreditEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<CreditEntity>().ToListAsync(cancellationToken);
    }
    public async Task<CreditEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Set<CreditEntity>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task AddAsync(CreditEntity credit, CancellationToken cancellationToken)
    {
        await _context.Set<CreditEntity>().AddAsync(credit, cancellationToken);
    }

    public async Task UpdateAsync(CreditEntity credit, CancellationToken cancellationToken)
    {
        _context.Set<CreditEntity>().Update(credit);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Set<CreditEntity>().FindAsync(new object[] { id }, cancellationToken);
        if (credit is not null) _context.Set<CreditEntity>().Remove(credit);
    }

    public async Task<decimal> GetRemainingCreditAsync(string id, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Set<CreditEntity>().FindAsync(new object[] { id }, cancellationToken);
        return credit?.CreditRemaining ?? 0;
    }

    public async Task UpdateRemainingCreditAsync(string id, decimal amount, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Set<CreditEntity>().FindAsync(new object[] { id }, cancellationToken);
        if (credit is not null)
        {
            credit.CreditRemaining = amount;
            _context.Set<CreditEntity>().Update(credit);
        }
    }
}
