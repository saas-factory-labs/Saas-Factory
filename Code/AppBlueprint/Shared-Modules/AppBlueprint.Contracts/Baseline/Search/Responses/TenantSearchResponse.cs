namespace AppBlueprint.Contracts.Baseline.Search.Responses;

public sealed class TenantSearchResponse
{
    public List<TenantSearchItem> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public long ExecutionTimeMs { get; init; }
    public string Query { get; init; } = string.Empty;
}

public sealed class TenantSearchItem
{
    public string Id { get; init; } = string.Empty;
    public string? Name { get; init; }
    public string? Description { get; init; }
    public float RelevanceScore { get; init; }
    public string? Headline { get; init; }
    public List<string> MatchedTerms { get; init; } = [];
}
