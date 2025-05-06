// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.UnitOfWork;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline
// {
//     [Authorize]
//
//     [ApiController]
//     [Route("api/search")]
//     [Produces("application/json")]
//     public class SearchController : ControllerBase
//     {
//         private readonly ISearchRepository _searchRepository;
//         private readonly IUnitOfWork _unitOfWork;
//
//         public SearchController(ISearchRepository searchRepository, IUnitOfWork unitOfWork)
//         {
//             _searchRepository = searchRepository ?? throw new ArgumentNullException(nameof(searchRepository));
//             _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//         }
//
//         /// <summary>
//         /// Gets all search results.
//         /// </summary>
//         /// <returns>List of search results</returns>
//         [HttpGet]
//         [ProducesResponseType(typeof(IEnumerable<SearchResponseDto>), StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status404NotFound)]
//         public async Task<ActionResult<IEnumerable<SearchResponseDto>>> Get(CancellationToken cancellationToken)
//         {
//             var searches = await _searchRepository.GetAllAsync(cancellationToken);
//             if (!searches.Any()) return NotFound(new { Message = "No searches found." });
//
//             var response = searches.Select(search => new SearchResponseDto
//             {
//                 Id = search.Id,
//                 Name = search.Name,
//                 // Email = search.Email
//             });
//
//             return Ok(response);
//         }
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

namespace AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
