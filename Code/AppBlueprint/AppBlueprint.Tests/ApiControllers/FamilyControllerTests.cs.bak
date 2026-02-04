// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2C.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using AppBlueprint.Presentation.ApiModule.Controllers.B2B;
// using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using Testcontainers.Container.Abstractions.Hosting;
// using Testcontainers.Container.Database.PostgreSql;
// using Testcontainers.PostgreSql;
//
// public class FamilyControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     // private readonly FamilyController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public FamilyControllerTests()
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
//         var familyRepository = new FamilyRepository(context);
//         _controller = new FamilyController(familyRepository);
//     }
//
//     [Fact]
//     public async Task GetFamilies_ReturnsOk_WhenFamiliesExist()
//     {
//         // Arrange
//         var families = new List<FamilyEntity>
//         {
//             new FamilyEntity { Id = 1, Name = "Family1" },
//             new FamilyEntity { Id = 2, Name = "Family2" }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Families.AddRange(families);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetFamilies(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedFamilies = Assert.IsAssignableFrom<IEnumerable<FamilyEntity>>(okResult.Value);
//         Assert.Equal(2, returnedFamilies.Count());
//     }
//
//     [Fact]
//     public async Task GetFamilies_ReturnsNotFound_WhenNoFamiliesExist()
//     {
//         // Act
//         var result = await _controller.GetFamilies(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetFamily_ReturnsOk_WhenFamilyExists()
//     {
//         // Arrange
//         var family = new FamilyEntity { Id = 1, Name = "Family1" };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Families.Add(family);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetFamily(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedFamily = Assert.IsType<FamilyEntity>(okResult.Value);
//         Assert.Equal("Family1", returnedFamily.Name);
//     }
//
//     [Fact]
//     public async Task GetFamily_ReturnsNotFound_WhenFamilyDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetFamily(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



