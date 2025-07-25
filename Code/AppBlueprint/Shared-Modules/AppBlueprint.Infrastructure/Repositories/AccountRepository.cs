using AppBlueprint.Infrastructure.DatabaseContexts;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    public async Task<AccountEntity> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken) ?? new AccountEntity
        {
            Name = "Not Found",
            IsActive = false,
            CreatedAt = DateTime.Now,
            Email = "Not Found",
            TenantId = "not-found",
            UserId = "not-found",
            Owner = CreateNotFoundUserEntity()
        };
    }
    public async Task<AccountEntity> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Accounts.FindAsync(new object?[] { slug, cancellationToken }, cancellationToken: cancellationToken) ?? new AccountEntity
        {
            Name = "Not Found",
            IsActive = false,
            CreatedAt = DateTime.Now,
            Email = "Not Found",
            TenantId = "not-found",
            UserId = "not-found",
            Owner = CreateNotFoundUserEntity()
        };
    }

    public async Task AddAsync(AccountEntity account, CancellationToken cancellationToken)
    {
        await _context.Accounts.AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AccountEntity account, CancellationToken cancellationToken)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken);
        if (account is not null)
        {
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    private static UserEntity CreateNotFoundUserEntity()
    {
        // Create a dummy user first to satisfy ProfileEntity requirement
        var tempUser = new UserEntity
        {
            FirstName = "temp",
            LastName = "temp",
            Email = "temp",
            UserName = "temp",
            Profile = new ProfileEntity { User = null! } // Temporarily use null!
        };

        // Now create the actual profile with the real user reference
        var profile = new ProfileEntity { User = tempUser };

        // Update the user with correct values and profile
        tempUser.FirstName = "Not Found";
        tempUser.LastName = "Not Found";
        tempUser.Email = "Not Found";
        tempUser.UserName = "Not Found";
        tempUser.IsActive = false;
        tempUser.CreatedAt = DateTime.Now;
        tempUser.Profile = profile;

        return tempUser;
    }
}
