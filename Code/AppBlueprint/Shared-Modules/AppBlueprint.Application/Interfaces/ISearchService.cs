namespace AppBlueprint.Application.Interfaces;

/// <summary>
/// Service interface for full-text search operations on entities.
/// Supports PostgreSQL full-text search with tenant isolation.
/// </summary>
/// <typeparam name="TEntity">The entity type to search</typeparam>
public interface ISearchService<TEntity> where TEntity : class
{
    /// <summary>
    /// Performs a full-text search on the specified entity type.
    /// Automatically respects tenant isolation for ITenantScoped entities.
    /// </summary>
    /// <param name="query">The search query with filters and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results with matched entities and metadata</returns>
    Task<SearchResult<TEntity>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the search index for a specific entity.
    /// Note: PostgreSQL tsvector columns are automatically updated via computed columns.
    /// This method is provided for compatibility with external search engines.
    /// </summary>
    /// <param name="entity">The entity to reindex</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReindexAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a bulk reindex of multiple entities.
    /// Note: PostgreSQL tsvector columns are automatically updated via computed columns.
    /// This method is provided for compatibility with external search engines.
    /// </summary>
    /// <param name="entities">The entities to reindex</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ReindexBulkAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}
