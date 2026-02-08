using DeploymentManager.ApiService.Domain.Entities;

namespace DeploymentManager.ApiService.Domain.Interfaces;

public interface IProjectRepository
{
    ProjectEntity? GetById(int id);
    IEnumerable<ProjectEntity> GetAll();
    void Add(ProjectEntity project);
    void Update(ProjectEntity project);
    void Delete(int id);
}
