using System.ComponentModel.DataAnnotations;
using AppBlueprint.Application.Attributes;
using AppBlueprint.Application.Enums;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using AppBlueprint.SharedKernel.Enums;
using AppBlueprint.SharedKernel;
using System.Globalization;

namespace AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Customer;

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
