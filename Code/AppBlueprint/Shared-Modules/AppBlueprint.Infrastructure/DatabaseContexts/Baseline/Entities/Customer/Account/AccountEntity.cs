using System.Globalization;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel;
using AppBlueprint.SharedKernel.Attributes;
using AppBlueprint.SharedKernel.Enums;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer.Account;

public class AccountEntity : BaseEntity, ITenantScoped
{
    public AccountEntity()
    {
        Id = PrefixedUlid.Generate("acc");
    }
    public CustomerType CustomerType { get; set; } // B2B / B2C / Government

    public required string
        Name
    {
        get;
        set;
    } // B2B (company name), Government ( government institution name), B2C (first name and last name of personal customer)

    [DataClassification(GDPRType.DirectlyIdentifiable)]
    public required string Email { get; set; } // GDPR data    

    public string? Role { get; set; }
    public bool IsActive { get; set; }

    public required UserEntity Owner { get; set; }
    public required string UserId { get; set; }
    public required string TenantId { get; set; }

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        // Generate slug based on account name
        // use regex to remove special characters

        // customertype
        // B2B - company name

        CultureInfo cultureInfo = CultureInfo.InvariantCulture;
        string slug = CustomerType.ToString().ToLower(cultureInfo) + "-" + Name.ToLower(cultureInfo);

        return slug;
    }
}
