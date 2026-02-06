using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Contracts.Baseline.Account.Requests;

public class GetAllAccountsRequest
{
    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string Email { get; set; } // GDPR data 

    public CustomerType CustomerType { get; set; } // B2B / B2C / Government

    public required string
        Name
    {
        get;
        set;
    } // B2B (company name), Government ( government institution name), B2C (first name and last name of personal customer)
}
