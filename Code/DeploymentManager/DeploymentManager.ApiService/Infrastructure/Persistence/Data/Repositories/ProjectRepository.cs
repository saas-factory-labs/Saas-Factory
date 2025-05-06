using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Persistence.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly List<ProjectEntity> _projects = new();

    public ProjectEntity GetById(int id)
    {
        return _projects.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<ProjectEntity> GetAll()
    {
        return _projects;
    }

    public void Add(ProjectEntity project)
    {
        _projects.Add(project);
    }

    public void Update(ProjectEntity project)
    {
        int index = _projects.FindIndex(p => p.Id == project.Id);
        if (index != -1) _projects[index] = project;
    }

    public void Delete(int id)
    {
        ProjectEntity? product = GetById(id);
        if (product is not null) _projects.Remove(product);
    }
}

//ProjectEntity project = new()
//{
//    //Id = _projects.Count + 1,
//    //Name = project.Name,
//    //Description = projectDTO.Description,
//    //CreatedAt = DateTime.Now,
//    //UpdatedAt = DateTime.Now
//};
