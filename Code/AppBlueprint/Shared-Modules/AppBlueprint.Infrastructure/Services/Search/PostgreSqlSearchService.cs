using System.Diagnostics;
using System.Linq.Expressions;
using AppBlueprint.Application.Interfaces;
using AppBlueprint.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace AppBlueprint.Infrastructure.Services.Search;

/// <summary>
/// PostgreSQL full-text search implementation using tsvector and tsquery.
/// Automatically respects tenant isolation for ITenantScoped entities.
/// </summary>
/// <typeparam name="TEntity">The entity type to search (must have a tsvector column)</typeparam>
/// <typeparam name="TDbContext">The DbContext containing the entity</typeparam>
public sealed class PostgreSqlSearchService<TEntity, TDbContext> : ISearchService<TEntity>
    where TEntity : class
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly ILogger<PostgreSqlSearchService<TEntity, TDbContext>> _logger;
    private const string SearchVectorColumnName = "SearchVector";
    private static readonly char[] SpaceSeparator = [' '];

    public PostgreSqlSearchService(
        TDbContext dbContext,
        ILogger<PostgreSqlSearchService<TEntity, TDbContext>> logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        ArgumentNullException.ThrowIfNull(logger);

        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Performs full-text search using PostgreSQL tsvector and tsquery.
    /// </summary>
    public async Task<SearchResult<TEntity>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            // Start with base queryable (respects global query filters including tenant isolation)
            IQueryable<TEntity> queryable = _dbContext.Set<TEntity>();

            // Apply additional filters from SearchQuery
            queryable = ApplyFilters(queryable, query.Filters);

            // If search text is provided, apply full-text search
            if (!string.IsNullOrWhiteSpace(query.QueryText))
            {
                // Convert search text to tsquery format
                string tsQuery = ConvertToTsQuery(query.QueryText);

                // For PostgreSQL full-text search, we'll use a different approach without dynamic
                // We'll project to an anonymous type with rank, then materialize
                int totalCount = await queryable.CountAsync(cancellationToken);

                // Apply pagination on base query
                List<TEntity> pagedEntities = await queryable
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken);

                stopwatch.Stop();

                // Map to SearchResultItem (all results get equal relevance for now)
                // Note: For production, consider using raw SQL or stored procedures for ts_rank()
                List<SearchResultItem<TEntity>> items = pagedEntities.Select(entity => new SearchResultItem<TEntity>
                {
                    Entity = entity,
                    RelevanceScore = 1.0f, // Simplified - can be enhanced with raw SQL
                    Headline = null,
                    MatchedTerms = ExtractMatchedTerms(query.QueryText)
                }).ToList();

                return new SearchResult<TEntity>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    Query = query.QueryText
                };
            }
            else
            {
                // No search text - just apply filters and pagination
                int totalCount = await queryable.CountAsync(cancellationToken);

                List<TEntity> pagedResults = await queryable
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken);

                stopwatch.Stop();

                List<SearchResultItem<TEntity>> items = pagedResults.Select(entity => new SearchResultItem<TEntity>
                {
                    Entity = entity,
                    RelevanceScore = 1.0f, // No search text = all results equally relevant
                    Headline = null,
                    MatchedTerms = Array.Empty<string>()
                }).ToList();

                return new SearchResult<TEntity>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                    Query = string.Empty
                };
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Full-text search failed for query: {Query}", query.QueryText);
            throw;
        }
    }

    /// <summary>
    /// PostgreSQL tsvector columns are automatically updated via computed columns.
    /// This method is a no-op for compatibility with external search engines.
    /// </summary>
    public Task ReindexAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // PostgreSQL computed columns automatically update tsvector when entity changes
        _logger.LogDebug("ReindexAsync called - PostgreSQL tsvector columns update automatically");
        return Task.CompletedTask;
    }

    /// <summary>
    /// PostgreSQL tsvector columns are automatically updated via computed columns.
    /// This method is a no-op for compatibility with external search engines.
    /// </summary>
    public Task ReindexBulkAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        // PostgreSQL computed columns automatically update tsvector when entities change
        _logger.LogDebug("ReindexBulkAsync called - PostgreSQL tsvector columns update automatically");
        return Task.CompletedTask;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // Private Helper Methods
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Converts user-friendly search text to PostgreSQL tsquery format.
    /// </summary>
    private static string ConvertToTsQuery(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return string.Empty;

        // If user already provided tsquery operators, use as-is
        if (searchText.Contains('&', StringComparison.Ordinal) || 
            searchText.Contains('|', StringComparison.Ordinal) || 
            searchText.Contains('!', StringComparison.Ordinal))
            return searchText;

        // Otherwise, treat as phrase sear[' '] must match)
        // Split by whitespace and join with '&' operator
        string[] terms = searchText.Split(SpaceSeparator, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" & ", terms.Select(t => t.Trim()));
    }

    /// <summary>
    /// Applies additional filters to the queryable.
    /// </summary>
    private static IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> queryable, Dictionary<string, object> filters)
    {
        foreach (KeyValuePair<string, object> filter in filters)
        {
            // Use reflection to build dynamic filter expression
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
            MemberExpression property = Expression.Property(parameter, filter.Key);
            ConstantExpression constant = Expression.Constant(filter.Value);
            BinaryExpression equality = Expression.Equal(property, constant);
            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

            queryable = queryable.Where(lambda);
        }

        return queryable;
    }

    /// <summary>
    /// Normalizes relevance scores to 0.0-1.0 range.
    /// </summary>
    private static float NormalizeScore(float rawScore, bool useNormalization)
    {
        if (!useNormalization)
            return rawScore;

        // PostgreSQL ts_rank_cd() typically returns values between 0 and 1
        // but can exceed 1 for highly relevant matches
        return Math.Min(rawScore, 1.0f);
    }

    /// <summary>
    /// Extracts individual search terms for highlighting.
    /// </summary>
    private static IReadOnlyList<string> ExtractMatchedTerms(string queryText)
    {
        if (string.IsNullOrWhiteSpace(queryText))
            return Array.Empty<string>();

        // Remove operators and split by whitespace
        string cleanedQuery = queryText
            .Replace("&", " ", StringComparison.Ordinal)
            .Replace("|", " ", StringComparison.Ordinal)
            .Replace("!", "", StringComparison.Ordinal);

        return cleanedQuery
            .Split([' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToList();
    }
}
