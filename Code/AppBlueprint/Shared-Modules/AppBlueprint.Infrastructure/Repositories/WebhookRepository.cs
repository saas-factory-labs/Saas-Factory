// using Appblueprint.SharedKernel.Models.B2B;
// using AppBlueprint.SharedKernel.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using AppBlueprint.SharedKernel.DatabaseContexts;
// using AppBlueprint.SharedKernel.DatabaseContexts.B2B.Models;
// using AppBlueprint.SharedKernel.DatabaseContexts.Baseline.Entities;
//
// namespace AppBlueprint.SharedKernel.Repositories
// {
//     public class WebhookRepository : IWebhookRepository
//     {
//         private readonly ApplicationDBContext _context;
//
//         public WebhookRepository(ApplicationDBContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<IEnumerable<WebhookEntity>> GetAllAsync()
//         {
//             return await _context.Set<WebhookEntity>().ToListAsync();
//         }
//
//         public async Task<WebhookEntity> GetByIdAsync(int id)
//         {
//             return await _context.Set<WebhookEntity>().FindAsync(id);
//         }
//
//         public async Task AddAsync(WebhookEntity tenant)
//         {
//             await _context.Set<WebhookEntity>().AddAsync(tenant);
//         }
//
//         public void Update(TenantEntity tenant)
//         {
//             _context.Set<TenantEntity>().Update(tenant);
//         }
//
//         public void Delete(int id)
//         {
//             var tenant = _context.Set<TenantEntity>().Find(id);
//             if (tenant is not null)
//             {
//                 _context.Set<TenantEntity>().Remove(tenant);
//             }
//         }
//     }
// }



