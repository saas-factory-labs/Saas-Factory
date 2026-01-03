namespace AppBlueprint.Web.Models.Auth;

public class SignupSessionData
{
    public string AccountType { get; set; } = string.Empty; // "personal" or "business"
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? VatNumber { get; set; }
    public string? Country { get; set; }
    public bool AcceptedTerms { get; set; }
    public DateTime CreatedAt { get; set; }
}
