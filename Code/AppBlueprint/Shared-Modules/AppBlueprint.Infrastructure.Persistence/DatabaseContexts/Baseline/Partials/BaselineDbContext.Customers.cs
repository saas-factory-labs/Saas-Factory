using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Account;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline;

public partial class BaselineDbContext
{
    public DbSet<CustomerEntity> Customers { get; set; }
    public DbSet<ContactPersonEntity> ContactPersons { get; set; }
    public DbSet<AccountEntity> Accounts { get; set; }

    partial void OnModelCreating_Customers(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContactPersonEntityConfiguration());
    }
}
