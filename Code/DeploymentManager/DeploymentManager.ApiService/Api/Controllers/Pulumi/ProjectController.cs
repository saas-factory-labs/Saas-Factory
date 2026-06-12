using AppBlueprint.Application.Constants;
using DeploymentManager.ApiService.Domain.DTOs.Project;
using DeploymentManager.ApiService.Domain.Entities;
using DeploymentManager.ApiService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentManager.ApiService.Api.Controllers.Pulumi;

public class ResponseDto
{
    public string Name { get; set; }
}

[Authorize(Roles = Roles.DeploymentManagerAdmin)]
[ApiController]
[Route("[controller]")]
public class ProjectController : Controller
{
    private readonly IInfrastructureCodeProvider _infrastructureCodeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectController(IUnitOfWork unitOfWork, IInfrastructureCodeProvider infrastructureCodeProvider)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(infrastructureCodeProvider);
        _unitOfWork = unitOfWork;
        _infrastructureCodeProvider = infrastructureCodeProvider;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectEntity>> Get(CancellationToken cancellationToken)
    {
        return await _unitOfWork.ProjectRepository.GetAllAsync(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(int id, CancellationToken cancellationToken)
    {
        ProjectEntity? project = await _unitOfWork.ProjectRepository.GetByIdAsync(id, cancellationToken);
        if (project is null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProjectEntity project, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        ProjectRequestDto projectDto = new() { Name = project.Name };

        Task<bool>? result = _infrastructureCodeProvider.CreateOrUpdateProject(projectDto);
        if (result is null) return new ObjectResult("Error creating project") { StatusCode = 500 };

        await _unitOfWork.ProjectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] ProjectEntity project, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _unitOfWork.ProjectRepository.UpdateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _unitOfWork.ProjectRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
