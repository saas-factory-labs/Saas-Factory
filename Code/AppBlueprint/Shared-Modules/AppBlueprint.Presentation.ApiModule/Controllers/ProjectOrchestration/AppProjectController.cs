// using AppBlueprint.Infrastructure.DatabaseContexts.TenantCatalog.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Infrastructure.UnitOfWork;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
//
// namespace AppBlueprint.Presentation.ApiModule.Controllers.ProjectOrchestration;
//
// [Authorize]
// [ApiController]
// [Route("[controller]")]
// [Produces("application/json")]
// public class AppProjectController : Controller
// {
//     private readonly IAppProjectRepository _appProjectRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IConfiguration _configuration;
//
//     public AppProjectController(
//         IAppProjectRepository appProjectRepository,
//         IUnitOfWork unitOfWork,
//         IConfiguration configuration)
//     {
//         _appProjectRepository = appProjectRepository ?? throw new ArgumentNullException(nameof(appProjectRepository));
//         _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//         _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
//     }
//
//     /// <summary>
//     /// Gets all AppProjects.
//     /// </summary>
//     /// <param name="cancellationToken">Cancellation token.</param>
//     /// <returns>List of AppProjects</returns>
//     [HttpGet("GetAppProjects")]
//     [ProducesResponseType(typeof(IEnumerable<AppProjectEntity>), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]    
//     public async Task<ActionResult> GetAppProjects([FromHeader(Name = "Authorization")] string authorizationHeader, CancellationToken cancellationToken)    {
//         var appProjects = await _appProjectRepository.GetAllAsync(cancellationToken);        
//         if (!appProjects.Any()) return NotFound(new { Message = "No app projects found." });
//         
//         // map to dto
//         var appProjectDTOs = appProjects.Select(appProject => new
//         {
//             Id = appProject.Id,
//         }).ToList();
//
//         return Ok(appProjectDTOs); 
//     }
//     
//     /// <summary>
//     /// Gets an AppProject by ID.
//     /// </summary>
//     /// <param name="id">AppProject ID.</param>
//     /// <returns>AppProject</returns>
//     /// GET: /AppProject/1
//     [HttpGet("GetAppProject/{id}")]
//     [ProducesResponseType(typeof(IEnumerable<AppProjectEntity>), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]  
//     public async Task<ActionResult> GetAppProject(int id, CancellationToken cancellationToken)
//     {
//         var appProject = await _appProjectRepository.GetByIdAsync(id, cancellationToken);
//         if (appProject is null)
//         {
//             return NotFound(new { Message = $"App Project with ID {id} not found." });
//         }       
//     
//         var appProjectDTO = new
//         {
//             Id = appProject.Id,
//         };
//         
//         return Ok(appProjectDTO);
//     }
//
//     /// <summary>
//     /// Creates a new AppProject.
//     /// </summary>
//     /// <param name="App Project">AppProject entity.</param>
//     /// <returns>Created AppProject.</returns>
//     /// POST: /AppProject
//     [HttpPost("CreateAppProject")]
//     [ProducesResponseType(typeof(IEnumerable<AppProjectEntity>), StatusCodes.Status201Created)]      
//     public async Task<ActionResult> CreateAppProject([FromBody] AppProjectEntity AppProject, CancellationToken cancellationToken)
//     {
//         if (!ModelState.IsValid) return BadRequest(ModelState);        
//     
//         // await _unitOfWork.AppProjectRepository.AddAsync(AppProject, cancellationToken);
//         await _unitOfWork.SaveChangesAsync();
//     
//         return CreatedAtAction("Get", new { id = AppProject.Id }, AppProject);
//     }
//
//     /// <summary>
//     /// Updates an existing AppProject.
//     /// </summary>
//     /// <param name="id">AppProject ID.</param>
//     /// <param name="AppProject">AppProject entity.</param>
//     /// <returns>No content.</returns>
//     /// PUT: /AppProject/1
//     [Authorize]
//     [HttpPut("UpdateAppProject/{id}")]
//     [ProducesResponseType(typeof(IEnumerable<AppProjectEntity>), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]  
//     public async Task<ActionResult<AppProjectEntity>> UpdateAppProject(int id, [FromBody] AppProjectEntity AppProject, CancellationToken cancellationToken)
//     {
//         if (!ModelState.IsValid) return BadRequest(ModelState);        
//         
//         await _unitOfWork.AppProjectRepository.UpdateAsync(AppProject, cancellationToken);
//         await _unitOfWork.SaveChangesAsync();
//         
//         return NoContent();
//     }
//
//     /// <summary>
//     /// Deletes an AppProject by ID.
//     /// </summary>
//     /// <param name="id">AppProject ID.</param>
//     /// <returns>No content.</returns>
//     /// DELETE: /AppProject/1
//     [HttpDelete("DeleteAppProject/{id}")]
//     [ProducesResponseType(typeof(IEnumerable<AppProjectEntity>), StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status404NotFound)]  
//     public async Task<ActionResult> DeleteAppProject(int id, CancellationToken cancellationToken)
//     {
//         if (!ModelState.IsValid) return BadRequest(ModelState);
//         await  _unitOfWork.AppProjectRepository.DeleteAsync(id, cancellationToken);
//         await _unitOfWork.SaveChangesAsync();
//         
//         return NoContent();
//     }
// }



