using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

// using AppBlueprint.Infrastructure.DatabaseContexts;

namespace AppBlueprint.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        List<AccountEntity>? accounts = await _context.Accounts.ToListAsync(cancellationToken);
        return accounts;
    }

    public async Task<AccountEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FindAsync(id, cancellationToken) ?? new AccountEntity
        {
            Name = "Not Found",
            IsActive = false,
            CreatedAt = DateTime.Now,
            Email = "Not Found",
            Owner = new UserEntity
            {
                FirstName = "Not Found",
                LastName = "Not Found",
                Email = "Not Found",
                IsActive = false,
                CreatedAt = DateTime.Now,
                UserName = "Not Found",
                Profile = new ProfileEntity()
            }
        };
    }

    public async Task<AccountEntity> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FindAsync(slug, cancellationToken) ?? new AccountEntity
        {
            Name = "Not Found",
            IsActive = false,
            CreatedAt = DateTime.Now,
            Email = "Not Found",
            Owner = new UserEntity
            {
                FirstName = "Not Found",
                LastName = "Not Found",
                Email = "Not Found",
                IsActive = false,
                CreatedAt = DateTime.Now,
                UserName = "Not Found",
                Profile = new ProfileEntity()
            }
        };
    }

    public async Task AddAsync(AccountEntity account, CancellationToken cancellationToken)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken)
    {
        _context.Accounts.Update(account);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        AccountEntity? account = await _context.Accounts.FindAsync(id, cancellationToken);
        if (account is not null) _context.Accounts.Remove(account);
    }
}
