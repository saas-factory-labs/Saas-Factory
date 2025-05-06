// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using TestContainers.Container.Abstractions.Hosting;
// using Testcontainers.PostgreSql;
//
// public class IntegrationControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly IntegrationController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public IntegrationControllerTests()
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
//         var integrationRepository = new IntegrationRepository(context);
//         _controller = new IntegrationController(integrationRepository);
//     }
//
//     [Fact]
//     public async Task GetIntegrations_ReturnsOk_WhenIntegrationsExist()
//     {
//         // Arrange
//         var integrations = new List<IntegrationEntity>
//         {
//             new IntegrationEntity { Id = 1, Name = "Integration1" },
//             new IntegrationEntity { Id = 2, Name = "Integration2" }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Integrations.AddRange(integrations);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetIntegrations(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedIntegrations = Assert.IsAssignableFrom<IEnumerable<IntegrationEntity>>(okResult.Value);
//         Assert.Equal(2, returnedIntegrations.Count());
//     }
//
//     [Fact]
//     public async Task GetIntegrations_ReturnsNotFound_WhenNoIntegrationsExist()
//     {
//         // Act
//         var result = await _controller.GetIntegrations(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetIntegration_ReturnsOk_WhenIntegrationExists()
//     {
//         // Arrange
//         var integration = new IntegrationEntity { Id = 1, Name = "Integration1" };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Integrations.Add(integration);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetIntegration(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedIntegration = Assert.IsType<IntegrationEntity>(okResult.Value);
//         Assert.Equal("Integration1", returnedIntegration.Name);
//     }
//
//     [Fact]
//     public async Task GetIntegration_ReturnsNotFound_WhenIntegrationDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetIntegration(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



