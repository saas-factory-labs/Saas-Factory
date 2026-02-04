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
//
// public class TeamControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly TeamController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public TeamControllerTests()
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
//         var teamRepository = new TeamRepository(context);
//         _controller = new TeamController(teamRepository);
//     }
//
//     [Fact]
//     public async Task GetTeams_ReturnsOk_WhenTeamsExist()
//     {
//         // Arrange
//         var teams = new List<TeamEntity>
//         {
//             new TeamEntity { Id = 1, Name = "Team1", IsActive = true },
//             new TeamEntity { Id = 2, Name = "Team2", IsActive = true }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Teams.AddRange(teams);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetTeams(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedTeams = Assert.IsAssignableFrom<IEnumerable<TeamEntity>>(okResult.Value);
//         Assert.Equal(2, returnedTeams.Count());
//     }
//
//     [Fact]
//     public async Task GetTeams_ReturnsNotFound_WhenNoTeamsExist()
//     {
//         // Act
//         var result = await _controller.GetTeams(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetTeam_ReturnsOk_WhenTeamExists()
//     {
//         // Arrange
//         var team = new TeamEntity { Id = 1, Name = "Team1", IsActive = true };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Teams.Add(team);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetTeam(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedTeam = Assert.IsType<TeamEntity>(okResult.Value);
//         Assert.Equal("Team1", returnedTeam.Name);
//     }
//
//     [Fact]
//     public async Task GetTeam_ReturnsNotFound_WhenTeamDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetTeam(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



