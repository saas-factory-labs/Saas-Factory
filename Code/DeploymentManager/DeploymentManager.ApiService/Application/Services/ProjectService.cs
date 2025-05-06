using DeploymentPortal.ApiService.Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Data.UnitOfWork;

namespace DeploymentPortal.ApiService.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectEntity> CreateProjectAsync(ProjectEntity project)
    {
        _unitOfWork.ProjectRepository.Add(project);
        _unitOfWork.SaveChanges();
        return project;
    }

    public IEnumerable<ProjectEntity> GetAllProjects()
    {
        return _unitOfWork.ProjectRepository.GetAll();
    }

    public ProjectEntity GetProjectById(int id)
    {
        return _unitOfWork.ProjectRepository.GetById(id);
    }

    public void UpdateProject(ProjectEntity project)
    {
        _unitOfWork.ProjectRepository.Update(project);
        _unitOfWork.SaveChanges();
    }

    public void DeleteProject(int id)
    {
        _unitOfWork.ProjectRepository.Delete(id);
        _unitOfWork.SaveChanges();
    }
}
