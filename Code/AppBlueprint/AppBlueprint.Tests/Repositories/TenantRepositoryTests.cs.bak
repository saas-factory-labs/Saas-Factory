// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using TestContainers.Container.Abstractions.Hosting;
// using Testcontainers.PostgreSql;
// using Testcontainers.PostgreSql;
//
// public class TenantRepositoryTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public TenantRepositoryTests()
//     {
//         _postgreSqlContainer = new ContainerBuilder<PostgreSqlContainer>()
//             .ConfigureDatabaseConfiguration("postgres", "yourStrong(!)Password", "TestDb")
//             .Build();
//         _postgreSqlContainer.StartAsync().Wait();
//
//         _dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
//             .UseNpgsql(_postgreSqlContainer.GetConnectionString())
//             .Options;
//     }
//
//     [Fact]
//     public async Task GetAllAsync_ReturnsAllTenants()
//     {
//         // Arrange
//         using var context = new ApplicationDBContext(_dbContextOptions);
//         context.Tenants.Add(new TenantEntity { Id = 1, Name = "Tenant1", IsActive = true });
//         context.Tenants.Add(new TenantEntity { Id = 2, Name = "Tenant2", IsActive = false });
//         await context.SaveChangesAsync();
//
//         var repository = new TenantRepository(context);
//
//         // Act
//         var tenants = await repository.GetAllAsync(CancellationToken.None);
//
//         // Assert
//         Assert.NotNull(tenants);
//         Assert.Equal(2, tenants.Count());
//     }
//
//     [Fact]
//     public async Task GetByIdAsync_ReturnsTenant_WhenTenantExists()
//     {
//         // Arrange
//         using var context = new ApplicationDBContext(_dbContextOptions);
//         var tenant = new TenantEntity { Id = 1, Name = "Tenant1", IsActive = true };
//         context.Tenants.Add(tenant);
//         await context.SaveChangesAsync();
//
//         var repository = new TenantRepository(context);
//
//         // Act
//         var result = await repository.GetByIdAsync(1, CancellationToken.None);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(1, result.Id);
//     }
// }



