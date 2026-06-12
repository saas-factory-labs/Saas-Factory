// using Bogus;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Shared.Enums;
// using Shared.Models;
// using Xunit;
//
// using Appblueprint.SharedKernel.DatabaseContexts.Baseline;
// using AppBlueprint.SharedKernel.Models.Baseline;
// using AppBlueprint.SharedKernel.DatabaseContexts.B2B;
// using AppBlueprint.SharedKernel.DatabaseContexts.B2B.Models;
//
// // IClassFixture<DatabaseFixture> is used to share the same instance of the database and common tests between all test classes across all db contexts
// // TODO: [SFVDM-348] Add database fixture class to add common test methods and properties across all db contexts
// // TODO: [SFVDM-348] Setup shared common fake data generation for all db contexts to use in unit tests (e.g. Faker, Bogus, etc.)
//
//
// // this class is used to test inserting data into a database and verifying that the data was inserted correctly and that relationships are working as expected
//
// public class B2BDbContextTests
// {
//     private readonly B2BDBContext _context;
//     private readonly BaselineDBContext _baselineContext;
//     private readonly IConfiguration Configuration;
//
//     public B2BDbContextTests(IConfiguration configuration)
//     {
//         Configuration = configuration;
//     }
//
//     [Fact]
//     public void CanCreateB2BDbContext()
//     {
//         // Arrange
//         // using var dbContext = new B2BDbContext(Configuration);
//
//         // Act
//
//         // Assert
//         // Assert.NotNull(dbContext);
//     }
//     [Fact]
//     public void CanAddTeam()
//     {
//         var teamFaker = new Faker<TeamEntity>()
//                 .RuleFor(t => t.Name, f => f.Company.CompanyName()) // Generate a fake company name for the team name
//                 .RuleFor(t => t.CreatedAt, f => f.Date.Past()); // Generate a random date in the past for the created at date
//
//         var team = teamFaker.Generate();
//
//         _context.Teams.Add(team);
//         _context.SaveChanges();
//
//         Assert.Single(_context.Teams);
//     }
//
//     [Fact]
//     public void CanAddUserToTeam()
//     {
//         // Arrange
//
//         var teamFaker = new Faker<TeamEntity>()
//     .RuleFor(t => t.Name, f => f.Company.CompanyName()) // Generate a fake company name for the team name
//     .RuleFor(t => t.CreatedAt, f => f.Date.Past()); // Generate a random date in the past for the created at date
//
//         var team = teamFaker.Generate();
//
//         var userFaker = new Faker<UserEntity>()
//                 .RuleFor(u => u.UserName, f => f.Person.UserName); // Generate a fake name for the user
//                                                                    // .RuleFor(u => u.EmailAddresses[0], f => f.Person.Email); // Generate a fake email for the user
//
//         //_context.Teams.Add(team);
//         //_baselineContext.Users.Add(user);
//         //_context.SaveChanges();
//
//         //// Act
//         //team.Users.Add(user);
//         //_context.SaveChanges();
//
//         //// Assert
//         //Assert.Contains(user, team.Users);
//     }
//
//
//
//
// }
//
//
// // Add more test methods here to cover different scenarios and functionalities of the b2b DbContext
//
//
// // var options = new DbContextOptionsBuilder<B2BDbContext>()
// //     .UseInMemoryDatabase(databaseName: "Test_Database_Add_Team")
// //     .Options;
//
//
// // var team = new TeamModel { /* set properties here */ };
// // context.Teams.Add(team);
// // context.SaveChanges();
//
// // Assert.Single(context.Teams);



