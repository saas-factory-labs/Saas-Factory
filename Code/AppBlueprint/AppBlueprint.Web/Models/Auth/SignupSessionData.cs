namespace AppBlueprint.Web.Models.Auth;

/// <summary>
/// Session data stored in browser localStorage during signup flow.
/// Used to persist data across OAuth redirect flow.
/// </summary>
internal sealed class SignupSessionData
{
    /// <summary>
    /// The type of account being created ('personal' or 'business').
    /// </summary>
    public string AccountType { get; init; } = string.Empty;

    /// <summary>
    /// First name of the user.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Last name of the user.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Company name (for business accounts only).
    /// </summary>
    public string? CompanyName { get; init; }

    /// <summary>
    /// Country (for business accounts only).
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// VAT number (for business accounts only).
    /// </summary>
    public string? VatNumber { get; init; }

    /// <summary>
    /// Whether the user accepted terms and conditions.
    /// </summary>
    public bool AcceptTerms { get; init; }
}
