// using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.TenantCatalog;
// using AppBlueprint.Infrastructure.Persistence.DatabaseContexts.TenantCatalog.Entities;
// using AppBlueprint.Infrastructure.Persistence.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;
//
// namespace AppBlueprint.Infrastructure.Persistence.Repositories
// {
//         public class AppProjectRepository : IAppProjectRepository
//         {
//             private readonly CatalogDbContext _context;
//
//             public AppProjectRepository(CatalogDbContext context)
//             {
//                 _context = context;
//             }
//
//             public async Task<IEnumerable<AppProjectEntity>> GetAllAsync(CancellationToken cancellationToken)
//             {
//                 var AppProjects = await _context.AppProjects.ToListAsync(cancellationToken);                
//                 return AppProjects;
//             }
//
//             public async Task<AppProjectEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
//             {
//                 return await _context.AppProjects.FindAsync(id, cancellationToken) ?? new AppProjectEntity();
//             }
//
//             public async Task AddAsync(AppProjectEntity AppProject, CancellationToken cancellationToken)
//             {
//                 await _context.AppProjects.AddAsync(AppProject, cancellationToken);
//             }
//
//             public async Task UpdateAsync(AppProjectEntity AppProject, CancellationToken cancellationToken)
//             {
//                  _context.AppProjects.Update(AppProject);                 
//             }
//
//             public async Task DeleteAsync(int id, CancellationToken cancellationToken)
//             {
//                 var AppProject = await _context.AppProjects.FindAsync(id, cancellationToken);
//                 if (AppProject is not null) _context.AppProjects.Remove(AppProject);
//         }
//     }
// }



