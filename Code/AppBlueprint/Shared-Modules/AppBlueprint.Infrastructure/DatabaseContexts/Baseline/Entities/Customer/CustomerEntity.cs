using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

public class CustomerEntity
{
    public CustomerEntity()
    {
        Tenants = new List<TenantEntity>();
        ContactPersons = new List<ContactPersonEntity>();
    }

    public int Id { get; set; }

    public CustomerType CustomerType { get; set; }

    // if customer is private person
    //public PersonModel Person { get; set; }
    //public int PersonId { get; set; }

    // if customer is a company

    //public string Name { get; set; }
    //public string Email { get; set; }
    // 1, 2, 3, 4 ... 10
    public int CurrentlyAtOnboardingFlowStep { get; set; }

    //public int TenantId { get; set; }
    public List<TenantEntity> Tenants { get; set; }
    public string Type { get; set; } // Personal/Company

    public string VatNumber { get; set; }

    public string Country { get; set; }

    public List<ContactPersonEntity> ContactPersons { get; set; }

    public string StripeCustomerId { get; set; }
    public string StripeSubscriptionId { get; set; }

    // Vat/Tax Id
    public string VatId { get; set; }
}
