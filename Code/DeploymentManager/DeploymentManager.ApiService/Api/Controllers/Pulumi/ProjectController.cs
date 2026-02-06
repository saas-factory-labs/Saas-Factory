using DeploymentManager.ApiService.Domain.DTOs.Project;
using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentManager.ApiService.Api.Controllers.Pulumi;

public class ResponseDto
{
    public string Name { get; set; }
}

[ApiController]
[Route("[controller]")]
public class
    ProjectController : Controller // inherit base class with injected services (ILogger, pulumi automation api service)
{
    private readonly IInfrastructureCodeProvider _infrastructureCodeProvider;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectController(IProjectRepository projectRepository, IUnitOfWork unitOfWork,
        IInfrastructureCodeProvider infrastructureCodeProvider)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _infrastructureCodeProvider = infrastructureCodeProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectEntity>> Get()
    {
        IEnumerable<ProjectEntity>? projects = _projectRepository.GetAll();

        // mapster (best) or automapper
        IEnumerable<ResponseDto>? dto = projects.Select(p => new ResponseDto
        {
            Name = p.Name
        });

        // project entity => response dto (sanitized) - explicit mapping
        return projects;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id)
    {
        ProjectEntity? project = _projectRepository.GetById(id);
        if (project is null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public IActionResult Post([FromBody] ProjectEntity project)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ProjectRequestDto projectDto = new()
        {
            Name = project.Name
        };

        Task<bool>? result = _infrastructureCodeProvider.CreateOrUpdateProject(projectDto);
        if (result is null) return new ObjectResult("Error creating project") { StatusCode = 500 };

        _unitOfWork.ProjectRepository.Add(project);
        _unitOfWork.SaveChanges();

        return CreatedAtAction("Get", new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectEntity>> Put(int id, [FromBody] ProjectEntity project)
    {
        if (!ModelState.IsValid)
        {
            _projectRepository.Update(project);
            _unitOfWork.SaveChanges();


            _unitOfWork.ProjectRepository.Update(project);
            _unitOfWork.SaveChanges();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        if (!ModelState.IsValid) _unitOfWork.ProjectRepository.Delete(id);

        return NoContent();
    }
}

//using DeploymentPortal.ApiService.Controllers.Base;

//using DeploymentPortal.ApiService.Models;
/*

- get pulumi automation api service
- create or update project
- create or update stack
- create or update stack config
- create or update stack secrets
- create or update stack tags
- create or update stack policy
- create azure infrastructure using stack
- create postgresql database
- deploy application to project infrastructure

*/
// CustomerBaseController
//  /api/deployment
