using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

public sealed class CustomerEntity : BaseEntity
{
    private readonly List<TenantEntity> _tenants = new();
    private readonly List<ContactPersonEntity> _contactPersons = new();

    public CustomerEntity()
    {
        Id = PrefixedUlid.Generate("customer");
        Type = string.Empty;
        Country = string.Empty;
    }

    public CustomerType CustomerType { get; set; }

    // if customer is private person
    //public PersonModel Person { get; set; }
    //public string PersonId { get; set; }

    // if customer is a company

    // 1, 2, 3, 4 ... 10
    public int CurrentlyAtOnboardingFlowStep { get; set; }

    public IReadOnlyCollection<TenantEntity> Tenants => _tenants.AsReadOnly();

    public string? Type { get; set; } // Personal/Company

    public string? VatNumber { get; set; }

    public string? Country { get; set; }

    public IReadOnlyCollection<ContactPersonEntity> ContactPersons => _contactPersons.AsReadOnly();

    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }

    // Vat/Tax Id
    public string? VatId { get; set; }

    // Organization relationship for B2B scenarios
    public string? OrganizationId { get; set; }

    // Domain methods for controlled collection management
    public void AddTenant(TenantEntity tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        if (_tenants.Any(t => t.Id == tenant.Id))
            return; // Tenant already exists

        _tenants.Add(tenant);
    }

    public void RemoveTenant(TenantEntity tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);
        _tenants.Remove(tenant);
    }

    public void AddContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);

        if (_contactPersons.Any(cp => cp.Id == contactPerson.Id))
            return; // Contact person already exists

        _contactPersons.Add(contactPerson);
        contactPerson.CustomerId = Id;
    }

    public void RemoveContactPerson(ContactPersonEntity contactPerson)
    {
        ArgumentNullException.ThrowIfNull(contactPerson);
        _contactPersons.Remove(contactPerson);
    }
}
