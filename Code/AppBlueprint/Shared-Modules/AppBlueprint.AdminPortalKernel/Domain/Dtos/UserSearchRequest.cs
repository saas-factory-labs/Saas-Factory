namespace AppBlueprint.AdminPortalKernel.Domain.Dtos;

/// <summary>Filters for the user administration page.</summary>
public sealed class UserSearchRequest
{
    /// <summary>Case-insensitive match against email, username, first and last name.</summary>
    public string? SearchText { get; set; }

    /// <summary>Restricts results to a single tenant of the target app.</summary>
    public string? TenantId { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
