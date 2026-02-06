using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.PaymentProvider;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Billing.Subscription;
using AppBlueprint.Infrastructure.DatabaseContexts.Modules.Credit;
using Microsoft.EntityFrameworkCore;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Partials;

public partial class BaselineDbContext
{
    public DbSet<CreditEntity> Credits { get; set; }
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    public DbSet<PaymentProviderEntity> PaymentProviders { get; set; }

    // Implementation of the partial method declared in the main class
    partial void OnModelCreating_Payment(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CreditEntityConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentProviderEntityConfiguration());
    }
}
