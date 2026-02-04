namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Represents the results of a full-text search operation.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public sealed class SearchResult<TEntity> where TEntity : class
{
    /// <summary>
    /// The matched entities.
    /// </summary>
    public IReadOnlyList<SearchResultItem<TEntity>> Items { get; init; } = Array.Empty<SearchResultItem<TEntity>>();

    /// <summary>
    /// Total number of matching results (across all pages).
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Number of results per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Query execution time in milliseconds.
    /// </summary>
    public long ExecutionTimeMs { get; init; }

    /// <summary>
    /// The original search query.
    /// </summary>
    public string Query { get; init; } = string.Empty;
}

/// <summary>
/// Represents a single search result item with relevance scoring.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public sealed class SearchResultItem<TEntity> where TEntity : class
{
    /// <summary>
    /// The matched entity.
    /// </summary>
    public required TEntity Entity { get; init; }

    /// <summary>
    /// Relevance score (0.0 to 1.0, higher is more relevant).
    /// Calculated using PostgreSQL's ts_rank() or ts_rank_cd() function.
    /// </summary>
    public float RelevanceScore { get; init; }

    /// <summary>
    /// Optional highlighted snippet showing matched text.
    /// Generated using PostgreSQL's ts_headline() function.
    /// </summary>
    public string? Headline { get; init; }

    /// <summary>
    /// Matched search terms (for highlighting in UI).
    /// </summary>
    public IReadOnlyList<string> MatchedTerms { get; init; } = Array.Empty<string>();
}
