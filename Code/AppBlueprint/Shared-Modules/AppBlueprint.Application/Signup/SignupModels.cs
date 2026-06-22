namespace AppBlueprint.Application.Signup;

/// <summary>
/// Request model for secure signup operation.
/// </summary>
public sealed record SignupRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? CompanyName { get; init; }
    public string? Country { get; init; }
    public required bool AcceptTerms { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }

    /// <summary>
    /// Tenant type: Personal = B2C, Organization = B2B.
    /// </summary>
    public required SignupTenantType TenantType { get; init; }
}

/// <summary>
/// Trusted identity values derived from validated OIDC claims.
/// </summary>
public sealed record AuthenticatedSignupIdentity
{
    public required string Email { get; init; }
    public required string ExternalAuthId { get; init; }
}

/// <summary>
/// Supported tenant types for signup provisioning.
/// </summary>
public enum SignupTenantType
{
    Personal = 0,
    Organization = 1
}

/// <summary>
/// Result model for signup operation.
/// </summary>
public sealed record SignupResult
{
    public required bool Success { get; init; }
    public required string TenantId { get; init; }
    public required string UserId { get; init; }
    public required string ProfileId { get; init; }
    public required string Email { get; init; }
    public required DateTime CreatedAt { get; init; }
}
