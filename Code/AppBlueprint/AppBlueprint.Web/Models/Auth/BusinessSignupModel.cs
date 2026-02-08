using System.ComponentModel.DataAnnotations;

namespace AppBlueprint.Web.Models.Auth;

internal sealed class BusinessSignupModel
{
    [Required(ErrorMessage = "Company name is required")]
    [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
    public string CompanyName { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "VAT number cannot exceed 50 characters")]
    public string? VatNumber { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "You must accept the terms and conditions")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
    public bool AcceptTerms { get; set; }
}
