namespace AppBlueprint.Contracts.Baseline.Search.Responses;

public sealed class UserSearchResponse
{
    public List<UserSearchItem> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long ExecutionTimeMs { get; init; }
    public string Query { get; init; } = string.Empty;
}

public sealed class UserSearchItem
{
    public string Id { get; init; } = string.Empty;
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? UserName { get; init; }
    public string? Email { get; init; }
    public float RelevanceScore { get; init; }
    public string? Headline { get; init; }
    public List<string> MatchedTerms { get; init; } = [];
}
