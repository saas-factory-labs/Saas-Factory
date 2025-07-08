using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities.Tenant.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.ContactPerson;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

public class CustomerEntity : BaseEntity
{
    public CustomerEntity()
    {
        Id = PrefixedUlid.Generate("customer");
        Tenants = new List<TenantEntity>();
        ContactPersons = new List<ContactPersonEntity>();
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

    public List<TenantEntity> Tenants { get; set; }

    public string? Type { get; set; } // Personal/Company

    public string? VatNumber { get; set; }

    public string? Country { get; set; }

    public List<ContactPersonEntity> ContactPersons { get; set; }

    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }

    // Vat/Tax Id
    public string? VatId { get; set; }

    // Organization relationship for B2B scenarios
    public string? OrganizationId { get; set; }
}
