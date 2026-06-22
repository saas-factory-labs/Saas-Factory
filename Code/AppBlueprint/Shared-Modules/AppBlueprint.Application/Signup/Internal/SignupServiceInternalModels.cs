namespace AppBlueprint.Application.Signup.Internal;

/// <summary>
/// Internal result type for mapping stored procedure JSON output.
/// The stored procedure returns a single JSON column.
/// </summary>
internal sealed class SignupStoredProcedureResult
{
    /// <summary>
    /// The JSON result returned by create_tenant_and_user stored procedure.
    /// Maps to the single column returned by the function.
    /// </summary>
    public string JsonResult { get; init; } = string.Empty;
}

/// <summary>
/// Internal normalized signup request used when invoking the stored procedure.
/// </summary>
internal sealed record ValidatedSignupRequest
{
    public required string TenantId { get; init; }
    public required string TenantName { get; init; }
    public required string UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string ExternalAuthId { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public required SignupTenantType TenantType { get; init; }
}
