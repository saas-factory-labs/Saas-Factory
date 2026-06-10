using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using DeploymentManager.ApiService.Infrastructure.Persistence.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DeploymentManager.ApiService.Infrastructure.Persistence.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DeploymentManagerDbContext _context;

    public CustomerRepository(DeploymentManagerDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task<CustomerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.DmCustomers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.DmCustomers.ToListAsync(cancellationToken);

    public async Task AddAsync(CustomerEntity customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);
        await _context.DmCustomers.AddAsync(customer, cancellationToken);
    }

    public Task UpdateAsync(CustomerEntity customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);
        _context.DmCustomers.Update(customer);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        CustomerEntity? customer = await GetByIdAsync(id, cancellationToken);
        if (customer is not null)
            _context.DmCustomers.Remove(customer);
    }
}
