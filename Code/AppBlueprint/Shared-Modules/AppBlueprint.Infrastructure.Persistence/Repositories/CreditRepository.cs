using AppBlueprint.Infrastructure.Persistence.DatabaseContexts;
using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.Modules.Credit;
using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.Persistence.Repositories;

public class CreditRepository : ICreditRepository
{
    private readonly ApplicationDbContext _context;

    public CreditRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CreditEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Credits.ToListAsync(cancellationToken);
    }
    public async Task<CreditEntity?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Credits.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(CreditEntity credit, CancellationToken cancellationToken)
    {
        await _context.Credits.AddAsync(credit, cancellationToken);
    }

    public Task UpdateAsync(CreditEntity credit, CancellationToken cancellationToken)
    {
        _context.Credits.Update(credit);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Credits.FindAsync([id], cancellationToken);
        if (credit is not null) _context.Credits.Remove(credit);
    }

    public async Task<decimal> GetRemainingCreditAsync(string id, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Credits.FindAsync([id], cancellationToken);
        return credit?.CreditRemaining ?? 0;
    }

    public async Task UpdateRemainingCreditAsync(string id, decimal amount, CancellationToken cancellationToken)
    {
        CreditEntity? credit = await _context.Credits.FindAsync([id], cancellationToken);
        if (credit is not null)
        {
            credit.CreditRemaining = amount;
            _context.Credits.Update(credit);
        }
    }
}
