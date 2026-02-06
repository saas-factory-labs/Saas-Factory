namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Represents a full-text search query with pagination and filtering options.
/// </summary>
public sealed class SearchQuery
{
    /// <summary>
    /// The search text to match against indexed fields.
    /// Uses PostgreSQL full-text search syntax (supports AND, OR, NOT operators).
    /// </summary>
    /// <example>"rental apartment" matches documents containing both words</example>
    /// <example>"rental | apartment" matches documents containing either word</example>
    /// <example>"rental &amp; !apartment" matches documents with "rental" but not "apartment"</example>
    public string QueryText { get; set; } = string.Empty;

    /// <summary>
    /// Number of results to return per page.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Additional filters to apply (property name -> value).
    /// Example: ["IsActive"] = true, ["Category"] = "Rental"
    /// </summary>
    public Dictionary<string, object> Filters { get; set; } = [];

    /// <summary>
    /// Ranking options for relevance scoring.
    /// </summary>
    public SearchRankingOptions Ranking { get; set; } = SearchRankingOptions.Default;

    /// <summary>
    /// Minimum relevance score threshold (0.0 to 1.0).
    /// Results with lower scores will be excluded.
    /// </summary>
    public float MinRelevanceScore { get; set; } = 0.0f;

    /// <summary>
    /// Sort order for results.
    /// If null, results are sorted by relevance score (descending).
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (ascending or descending).
    /// Only applicable when SortBy is specified.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
}

/// <summary>
/// Ranking options for PostgreSQL full-text search.
/// Controls how relevance scores are calculated.
/// </summary>
public sealed class SearchRankingOptions
{
    /// <summary>
    /// Default ranking (balanced between precision and recall).
    /// </summary>
    public static readonly SearchRankingOptions Default = new()
    {
        UseNormalization = true,
        WeightLabels = [1.0f, 0.4f, 0.2f, 0.1f] // D, C, B, A
    };

    /// <summary>
    /// Whether to normalize ranks (0.0 to 1.0).
    /// Recommended for comparing scores across different queries.
    /// </summary>
    public bool UseNormalization { get; set; } = true;

    /// <summary>
    /// Weights for different label categories (D, C, B, A).
    /// PostgreSQL supports weighted search vectors with labels D (most important), C, B, A (least important).
    /// Default: D=1.0, C=0.4, B=0.2, A=0.1
    /// </summary>
    public float[] WeightLabels { get; set; } = [1.0f, 0.4f, 0.2f, 0.1f];
}

/// <summary>
/// Sort direction for search results.
/// </summary>
public enum SortDirection
{
    Ascending,
    Descending
}
