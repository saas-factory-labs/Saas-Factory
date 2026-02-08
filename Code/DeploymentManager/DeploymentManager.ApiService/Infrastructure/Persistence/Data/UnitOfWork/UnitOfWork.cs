// using Domain.Interfaces;
// using Infrastructure.Persistence.Data.Context;
// using Infrastructure.Persistence.Data.Repositories;
// using Infrastructure.Persistence.Data.UnitOfWork;

// public class UnitOfWork : IUnitOfWork
// {
//     private readonly DeploymentManagerContext _context;
//     private ProjectRepository _projectRepository;
//     // Add other repository fields here as needed

//     public UnitOfWork(DeploymentManagerContext context)
//     {
//         _context = context;
//     }

//     public IProjectRepository ProjectRepository
//     {
//         get
//         {
//             if (_projectRepository is null)
//             {
//                 _projectRepository = new ProjectRepository();
//             }
//             return _projectRepository;
//         }
//     }

//     // Implement the same pattern for other repositories here

//     public void SaveChanges()
//     {
//         _context.SaveChanges();
//     }

//     public void Dispose()
//     {
//         _context.Dispose();
//     }
// }



