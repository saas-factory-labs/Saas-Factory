// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Presentation.ApiModule.Controllers.B2B;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using TestContainers.Container.Abstractions.Hosting;
// using TestContainers.Container.Abstractions.Hosting;
// using Testcontainers.PostgreSql;
//
// public class TenantControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly TenantController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public TenantControllerTests()
//     {
//         _postgreSqlContainer = new ContainerBuilder<PostgreSqlContainer>()
//             .ConfigureDatabaseConfiguration("postgres", "yourStrong(!)Password", "TestDb")
//             .Build();
//         _postgreSqlContainer.StartAsync().Wait();
//
//         _dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
//             .UseNpgsql(_postgreSqlContainer.GetConnectionString())
//             .Options;
//
//         var context = new ApplicationDBContext(_dbContextOptions);
//         var tenantRepository = new TenantRepository(context);
//         _controller = new TenantController(tenantRepository);
//     }
//
//     [Fact]
//     public async Task GetTenants_ReturnsOk_WhenTenantsExist()
//     {
//         // Arrange
//         var tenants = new List<TenantEntity>
//         {
//             new TenantEntity { Id = 1, Name = "Tenant1", IsActive = true },
//             new TenantEntity { Id = 2, Name = "Tenant2", IsActive = false }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Tenants.AddRange(tenants);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetTenants(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedTenants = Assert.IsAssignableFrom<IEnumerable<TenantEntity>>(okResult.Value);
//         Assert.Equal(2, returnedTenants.Count());
//     }
//
//     [Fact]
//     public async Task GetTenants_ReturnsNotFound_WhenNoTenantsExist()
//     {
//         // Act
//         var result = await _controller.GetTenants(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetTenant_ReturnsOk_WhenTenantExists()
//     {
//         // Arrange
//         var tenant = new TenantEntity { Id = 1, Name = "Tenant1", IsActive = true };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Tenants.Add(tenant);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetTenant(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedTenant = Assert.IsType<TenantEntity>(okResult.Value);
//         Assert.Equal("Tenant1", returnedTenant.Name);
//     }
//
//     [Fact]
//     public async Task GetTenant_ReturnsNotFound_WhenTenantDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetTenant(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



