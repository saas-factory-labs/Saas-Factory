using Domain.Entities;

namespace Domain.Interfaces;

public interface IProjectRepository
{
    ProjectEntity GetById(int id);
    IEnumerable<ProjectEntity> GetAll();
    void Add(ProjectEntity project);
    void Update(ProjectEntity project);
    void Delete(int id);
}
