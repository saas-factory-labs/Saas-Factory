using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CustomerEntity customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(CustomerEntity customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
