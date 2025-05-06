using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.Customer.Requests;

public class CreateCustomerRequest(
    string name,
    IReadOnlyList<CreateTenantRequest> tenants,
    IReadOnlyList<CreateContactPersonRequest> contactPersons)
{
    public CustomerType CustomerType { get; set; }
    public string Name { get; set; } = name;

    public IReadOnlyList<CreateTenantRequest> Tenants { get; set; } = tenants;
    public IReadOnlyList<CreateContactPersonRequest> ContactPersons { get; set; } = contactPersons;
}
