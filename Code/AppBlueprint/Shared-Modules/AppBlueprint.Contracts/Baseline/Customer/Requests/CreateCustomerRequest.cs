using AppBlueprint.Contracts.B2B.Contracts.Tenant.Requests;
using AppBlueprint.Contracts.Baseline.ContactPerson.Requests;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.Customer.Requests;

/// <summary>
/// Request to create a new customer with associated tenants and contact persons.
/// </summary>
public class CreateCustomerRequest(
    string name,
    IReadOnlyList<CreateTenantRequest> tenants,
    IReadOnlyList<CreateContactPersonRequest> contactPersons)
{
    /// <summary>
    /// Type of customer (Individual, Business, etc.).
    /// </summary>
    public CustomerType CustomerType { get; set; }
    
    /// <summary>
    /// Name of the customer or business.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Tenants associated with this customer.
    /// </summary>
    public IReadOnlyList<CreateTenantRequest> Tenants { get; set; } = tenants;
    
    /// <summary>
    /// Contact persons associated with this customer.
    /// </summary>
    public IReadOnlyList<CreateContactPersonRequest> ContactPersons { get; set; } = contactPersons;
}
