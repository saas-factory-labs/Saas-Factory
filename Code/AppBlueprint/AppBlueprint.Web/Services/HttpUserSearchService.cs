using AppBlueprint.Api.Client.Sdk;
using AppBlueprint.Api.Client.Sdk.Models;
using AppBlueprint.Application.Interfaces;
using UserEntity = AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User.UserEntity;

namespace AppBlueprint.Web.Services;

/// <summary>
/// HTTP-based search service for users that delegates to the API service via the Kiota client.
/// </summary>
internal sealed class HttpUserSearchService : ISearchService<UserEntity>
{
    private readonly ApiClient _apiClient;
    private readonly ILogger<HttpUserSearchService> _logger;

    public HttpUserSearchService(ApiClient apiClient, ILogger<HttpUserSearchService> logger)
    {
        ArgumentNullException.ThrowIfNull(apiClient);
        ArgumentNullException.ThrowIfNull(logger);

        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<SearchResult<UserEntity>> SearchAsync(SearchQuery query, CancellationToken cancellationToken = default)
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

        UserSearchResponseDto? response = await _apiClient.Api.V1.Search.Users.PostAsync(requestDto, cancellationToken: cancellationToken);

        if (response is null)
        {
            _logger.LogWarning("User search API returned null response for query: {QueryText}", query.QueryText);
            return new SearchResult<UserEntity>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                Query = query.QueryText
            };
        }

        return new SearchResult<UserEntity>
        {
            Items = (response.Items ?? []).Select(item => new SearchResultItem<UserEntity>
            {
                Entity = new UserEntity
                {
                    Id = item.Id ?? string.Empty,
                    FirstName = item.FirstName ?? string.Empty,
                    LastName = item.LastName ?? string.Empty,
                    UserName = item.UserName ?? string.Empty,
                    Email = item.Email,
                    Profile = null!,
                    TenantId = string.Empty
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

    public Task ReindexAsync(UserEntity entity, CancellationToken cancellationToken = default)
    {
        // Reindexing is managed server-side via PostgreSQL computed tsvector columns.
        return Task.CompletedTask;
    }

    public Task ReindexBulkAsync(IEnumerable<UserEntity> entities, CancellationToken cancellationToken = default)
    {
        // Reindexing is managed server-side via PostgreSQL computed tsvector columns.
        return Task.CompletedTask;
    }
}
