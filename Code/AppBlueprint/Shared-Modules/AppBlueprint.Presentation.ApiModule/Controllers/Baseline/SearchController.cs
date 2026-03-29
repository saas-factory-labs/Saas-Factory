using AppBlueprint.Application.Interfaces;
using AppBlueprint.Contracts.Baseline.Search.Requests;
using AppBlueprint.Contracts.Baseline.Search.Responses;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.Tenant;
using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities.User;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/search")]
[Produces("application/json")]
public class SearchController : BaseController
{
    private readonly ISearchService<TenantEntity> _tenantSearchService;
    private readonly ISearchService<UserEntity> _userSearchService;

    public SearchController(
        IConfiguration configuration,
        ISearchService<TenantEntity> tenantSearchService,
        ISearchService<UserEntity> userSearchService)
        : base(configuration)
    {
        ArgumentNullException.ThrowIfNull(tenantSearchService);
        ArgumentNullException.ThrowIfNull(userSearchService);

        _tenantSearchService = tenantSearchService;
        _userSearchService = userSearchService;
    }

    /// <summary>
    ///     Performs a full-text search across tenants.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged search results for tenants.</returns>
    /// <response code="200">Returns the matching tenants.</response>
    [HttpPost(ApiEndpoints.Search.Tenants)]
    [ProducesResponseType(typeof(TenantSearchResponse), StatusCodes.Status200OK)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<TenantSearchResponse>> SearchTenants(
        [FromBody] SearchRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        SearchQuery query = MapToSearchQuery(request);
        SearchResult<TenantEntity> result = await _tenantSearchService.SearchAsync(query, cancellationToken);

        return Ok(new TenantSearchResponse
        {
            Items = result.Items.Select(item => new TenantSearchItem
            {
                Id = item.Entity.Id,
                Name = item.Entity.Name,
                Description = item.Entity.Description,
                RelevanceScore = item.RelevanceScore,
                Headline = item.Headline,
                MatchedTerms = item.MatchedTerms.ToList()
            }).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            ExecutionTimeMs = result.ExecutionTimeMs,
            Query = result.Query
        });
    }

    /// <summary>
    ///     Performs a full-text search across users.
    /// </summary>
    /// <param name="request">The search query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged search results for users.</returns>
    /// <response code="200">Returns the matching users.</response>
    [HttpPost(ApiEndpoints.Search.Users)]
    [ProducesResponseType(typeof(UserSearchResponse), StatusCodes.Status200OK)]
    [MapToApiVersion(ApiVersions.V1)]
    public async Task<ActionResult<UserSearchResponse>> SearchUsers(
        [FromBody] SearchRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        SearchQuery query = MapToSearchQuery(request);
        SearchResult<UserEntity> result = await _userSearchService.SearchAsync(query, cancellationToken);

        return Ok(new UserSearchResponse
        {
            Items = result.Items.Select(item => new UserSearchItem
            {
                Id = item.Entity.Id,
                FirstName = item.Entity.FirstName,
                LastName = item.Entity.LastName,
                UserName = item.Entity.UserName,
                Email = item.Entity.Email,
                RelevanceScore = item.RelevanceScore,
                Headline = item.Headline,
                MatchedTerms = item.MatchedTerms.ToList()
            }).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            ExecutionTimeMs = result.ExecutionTimeMs,
            Query = result.Query
        });
    }

    private static SearchQuery MapToSearchQuery(SearchRequest request)
    {
        return new SearchQuery
        {
            QueryText = request.QueryText,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            MinRelevanceScore = request.MinRelevanceScore,
            SortBy = request.SortBy,
            SortDirection = Enum.TryParse<SortDirection>(request.SortDirection, ignoreCase: true, out SortDirection dir)
                ? dir
                : SortDirection.Descending
        };
    }
}

//
//         /// <summary>
//         /// Gets a search result by ID.
//         /// </summary>
//         /// <param name="id">Search ID.</param>
//         /// <returns>Search result</returns>
//         [HttpGet("{id}")]
//         [ProducesResponseType(typeof(SearchResponseDto), StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public async Task<ActionResult<SearchResponseDto>> Get(int id, CancellationToken cancellationToken)
//         {
//             var search = await _searchRepository.GetByIdAsync(id, cancellationToken);
//             if (search is null) return NotFound(new { Message = $"Search with ID {id} not found." });
//
//             var response = new SearchResponseDto
//             {
//                 Id = search.Id,
//                 Name = search.Name,
//                 // Email = search.Email
//             };
//
//             return Ok(response);
//         }
//
//         /// <summary>
//         /// Creates a new search entry.
//         /// </summary>
//         /// <param name="searchDto">Search data transfer object.</param>
//         /// <returns>Created search entry.</returns>
//         [HttpPost]
//         [ProducesResponseType(typeof(SearchResponseDto), StatusCodes.Status201Created)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public async Task<ActionResult> Post([FromBody] SearchRequestDto searchDto, CancellationToken cancellationToken)
//         {
//             if (!ModelState.IsValid) return BadRequest(ModelState);
//
//             var newSearch = new SearchEntity
//             {
//                 Name = searchDto.Name,
//                 // Email = searchDto.Email
//             };
//
//             await _searchRepository.AddAsync(newSearch, cancellationToken);
//             await _unitOfWork.SaveChangesAsync();
//
//             return CreatedAtAction(nameof(Get), new { id = newSearch.Id }, newSearch);
//         }
//
//         /// <summary>
//         /// Updates an existing search entry.
//         /// </summary>
//         /// <param name="id">Search ID.</param>
//         /// <param name="searchDto">Search data transfer object.</param>
//         /// <returns>No content.</returns>
//         [HttpPut("{id}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public async Task<ActionResult> Put(int id, [FromBody] SearchRequestDto searchDto, CancellationToken cancellationToken)
//         {
//             var existingSearch = await _searchRepository.GetByIdAsync(id, cancellationToken);
//             if (existingSearch is null) return NotFound(new { Message = $"Search with ID {id} not found." });
//
//             existingSearch.Name = searchDto.Name;
//             // existingSearch.Email = searchDto.Email;
//
//             _searchRepository.Update(existingSearch);
//             await _unitOfWork.SaveChangesAsync();
//
//             return NoContent();
//         }
//
//         /// <summary>
//         /// Deletes a search entry by ID.
//         /// </summary>
//         /// <param name="id">Search ID.</param>
//         /// <returns>No content.</returns>
//         [HttpDelete("{id}")]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
//         {
//             var existingSearch = await _searchRepository.GetByIdAsync(id, cancellationToken);
//             if (existingSearch is null) return NotFound(new { Message = $"Search with ID {id} not found." });
//
//             _searchRepository.Delete(existingSearch);
//             await _unitOfWork.SaveChangesAsync();
//
//             return NoContent();
//         }
//     }
//
//     public interface ISearchRepository
//     {
//         Task<IEnumerable<SearchEntity>> GetAllAsync(CancellationToken cancellationToken);
//         Task<SearchEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
//         Task AddAsync(SearchEntity search, CancellationToken cancellationToken);
//         void Update(SearchEntity search);
//         void Delete(SearchEntity search);
//     }
//
//     public class SearchRequestDto
//     {
//         public string Name { get; set; }
//         public string Email { get; set; }
//     }
//
//     public class SearchResponseDto
//     {
//         public int Id { get; set; }
//         public string Name { get; set; }
//         public string Email { get; set; }
//     }
// }
