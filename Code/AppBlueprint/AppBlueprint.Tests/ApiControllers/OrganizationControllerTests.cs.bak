// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Presentation.ApiModule.Controllers.B2B;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using TestContainers.Container.Abstractions.Hosting;
// using TestContainers.Container.Abstractions.Hosting;
// using Testcontainers.PostgreSql;
//
// public class OrganizationControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly OrganizationController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public OrganizationControllerTests()
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
//         var organizationRepository = new OrganizationRepository(context);
//         _controller = new OrganizationController(organizationRepository);
//     }
//
//     [Fact]
//     public async Task GetOrganizations_ReturnsOk_WhenOrganizationsExist()
//     {
//         // Arrange
//         var organizations = new List<OrganizationEntity>
//         {
//             new OrganizationEntity { Id = 1, Name = "Org1" },
//             new OrganizationEntity { Id = 2, Name = "Org2" }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Organizations.AddRange(organizations);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetOrganizations(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedOrganizations = Assert.IsAssignableFrom<IEnumerable<OrganizationEntity>>(okResult.Value);
//         Assert.Equal(2, returnedOrganizations.Count());
//     }
//
//     [Fact]
//     public async Task GetOrganizations_ReturnsNotFound_WhenNoOrganizationsExist()
//     {
//         // Act
//         var result = await _controller.GetOrganizations(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetOrganization_ReturnsOk_WhenOrganizationExists()
//     {
//         // Arrange
//         var organization = new OrganizationEntity { Id = 1, Name = "Org1" };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Organizations.Add(organization);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetOrganization(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedOrganization = Assert.IsType<OrganizationEntity>(okResult.Value);
//         Assert.Equal("Org1", returnedOrganization.Name);
//     }
//
//     [Fact]
//     public async Task GetOrganization_ReturnsNotFound_WhenOrganizationDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetOrganization(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



