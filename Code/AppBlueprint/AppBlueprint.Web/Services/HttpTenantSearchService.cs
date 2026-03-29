using AppBlueprint.Api.Client.Sdk;
using AppBlueprint.Api.Client.Sdk.Models;
using AppBlueprint.Application.Interfaces;
using TenantEntity = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant.TenantEntity;

namespace AppBlueprint.Web.Services;

/// <summary>
/// HTTP-based search service for tenants that delegates to the API service via the Kiota client.
/// </summary>
internal sealed class HttpTenantSearchService : ISearchService<TenantEntity>
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<HttpTenantSearchService> _logger;

    public HttpTenantSearchService(ApiClient apiClient, ILogger<HttpTenantSearchService> logger)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(logger);

        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<SearchResult<TenantEntity>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var requestDto = new SearchRequestDto
        {
            QueryText = query.QueryText,
            PageSize = query.PageSize,
            PageNumber = query.PageNumber,
            MinRelevanceScore = query.MinRelevanceScore,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection.ToString()
        };

        TenantSearchResponseDto? response = await _apiClient.Api.V1.Search.Tenants.PostAsync(requestDto, cancellationToken: cancellationToken);

        if (response is null)
        {
            _logger.LogWarning("Tenant search API returned null response for query: {QueryText}", query.QueryText);
            return new SearchResult<TenantEntity>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Query = query.QueryText
            };
        }

        return new SearchResult<TenantEntity>
        {
            Items = (response.Items ?? []).Select(item => new SearchResultItem<TenantEntity>
            {
                Entity = new TenantEntity
                {
                    Id = item.Id ?? string.Empty,
                    Name = item.Name ?? string.Empty,
                    Description = item.Description
                },
                RelevanceScore = item.RelevanceScore ?? 0f,
                Headline = item.Headline,
                MatchedTerms = item.MatchedTerms ?? []
            }).ToList().AsReadOnly(),
            TotalCount = response.TotalCount ?? 0,
            PageNumber = response.PageNumber ?? query.PageNumber,
            PageSize = response.PageSize ?? query.PageSize,
            ExecutionTimeMs = response.ExecutionTimeMs ?? 0,
            Query = response.Query ?? query.QueryText
        };
    }

    public Task ReindexAsync(TenantEntity entity, CancellationToken cancellationToken = default)
    {
        // Reindexing is managed server-side via PostgreSQL computed tsvector columns.
        return Task.CompletedTask;
    }

    public Task ReindexBulkAsync(IEnumerable<TenantEntity> entities, CancellationToken cancellationToken = default)
    {
        // Reindexing is managed server-side via PostgreSQL computed tsvector columns.
        return Task.CompletedTask;
    }
}
