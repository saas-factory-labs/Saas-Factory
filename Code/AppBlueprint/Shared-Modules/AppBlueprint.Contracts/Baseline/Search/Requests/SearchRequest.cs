namespace AppBlueprint.Contracts.Baseline.Search.Requests;

public sealed class SearchRequest
{
    public string QueryText { get; set; } = string.Empty;

    public int PageSize { get; set; } = 20;

    public int PageNumber { get; set; } = 1;

    public float MinRelevanceScore { get; set; }

    public string? SortBy { get; set; }

    public string SortDirection { get; set; } = "Descending";
}
