// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Testcontainers.PostgreSql;
// using Assert = TUnit.Assertions.Assert;
//
// namespace AppBlueprint.Tests.Repositories;
//
// public class AccountRepositoryTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
//     private readonly IConfiguration _configuration;
//
//     public AccountRepositoryTests(IConfiguration configuration)
//     {
//         _configuration = configuration;
//
//         _postgreSqlContainer = new PostgreSqlBuilder()
//             .WithDatabase("TestDb")
//             .WithUsername("postgres")
//             .WithPassword("yourStrong(!)Password")
//             .Build();
//
//         _postgreSqlContainer.StartAsync(); // Start the container
//
//         _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseNpgsql(_postgreSqlContainer.GetConnectionString())
//             .Options;
//     }
//
//     [Test]
//     public async Task GetAllAsync_ReturnsAllAccounts()
//     {
//         // Arrange
//         using var context = new ApplicationDbContext(_dbContextOptions, configuration: _configuration);
//         context.Accounts.Add(new AccountEntity
//         {
//             AccountId = 1, Name = "adkakd", Owner = new UserEntity()
//             {
//                 Email = "adajdja@dkakd.dk",
//             },
//             Email = "test1@example.com", Role = "Admin"
//         });
//         context.Accounts.Add(new AccountEntity
//         {
//             AccountId = 1, Name = "adkakd", Owner = new UserEntity()
//             {
//                 Email = "adajdja@dkakd.dk",
//             },
//             Email = "test1@example.com", Role = "Admin"
//         });
//         await context.SaveChangesAsync();
//
//         var repository = new AccountRepository(context);
//
//         // Act
//         IEnumerable<AccountEntity>? accounts = await repository.GetAllAsync(CancellationToken.None);
//
//         // Assert
//         await Assert.That(accounts).IsNotNull();
//         await Assert.That(accounts.Count()).IsEqualTo(2);
//     }
//
//     [Test]
//     public async Task GetByIdAsync_ReturnsAccount_WhenAccountExists()
//     {
//         // Arrange
//         using var context = new ApplicationDbContext(_dbContextOptions, configuration: _configuration);
//         var account = new AccountEntity
//         {
//             AccountId = 1, Name = "adkakdka", Owner = new UserEntity()
//             {
//                 Email = "adkakdk@sjioajig.com"
//             },
//             Email = "test@example.com", Role = "Admin"
//         };
//         context.Accounts.Add(account);
//         await context.SaveChangesAsync();
//
//         var repository = new AccountRepository(context);
//
//         // Act
//         AccountEntity? result = await repository.GetByIdAsync(1, CancellationToken.None);
//
//         // Assert
//         await Assert.That(result).IsNotNull();
//         await Assert.That(result.AccountId).IsEqualTo(1);
//     }
// }
//
//
// // using AppBlueprint.Infrastructure.DatabaseContexts;
// // using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// // using AppBlueprint.Infrastructure.Repositories;
// // using Microsoft.EntityFrameworkCore;
// // using Microsoft.Extensions.Configuration;
// // using Testcontainers.PostgreSql;
// // //using Xunit;
// // // using TestContainers.Container.Abstractions;
// // using Assert = TUnit.Assertions.Assert;
// //
// // namespace AppBlueprint.Tests.Repositories;
// //
// // public class AccountRepositoryTests
// // {
// //     private readonly PostgreSqlContainer _postgreSqlContainer;
// //     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
// //     private readonly IConfiguration _configuration;
// //
// //     public AccountRepositoryTests(IConfiguration configuration)
// //     {
// //         _configuration = configuration;
// //         
// //         _postgreSqlContainer = new PostgreSqlBuilder()
// //             .WithDatabase("TestDb")
// //             .WithUsername("postgres")
// //             .WithPassword("yourStrong(!)Password")
// //             .Build();
// //
// //         _dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
// //             .UseNpgsql(_postgreSqlContainer.GetConnectionString())
// //             .Options;
// //     }
// //     
// //
// //     [Test]
// //     public async Task GetAllAsync_ReturnsAllAccounts()
// //     {
// //         // Arrange
// //         using var context = new ApplicationDBContext(_dbContextOptions, configuration: _configuration);
// //         context.Accounts.Add(new AccountEntity { AccountId = 1, Email = "test1@example.com", Role = "Admin" });
// //         context.Accounts.Add(new AccountEntity { AccountId = 2, Email = "test2@example.com", Role = "User" });
// //         await context.SaveChangesAsync();
// //
// //         var repository = new AccountRepository(context);
// //
// //         // Act
// //         var accounts = await repository.GetAllAsync(CancellationToken.None);
// //
// //         // Assert
// //         await Assert.That(accounts).IsNotNull();
// //         await Assert.That(accounts.Count()).IsEqualTo(2);
// //     }
// //
// //     [Test]
// //     public async Task GetById



