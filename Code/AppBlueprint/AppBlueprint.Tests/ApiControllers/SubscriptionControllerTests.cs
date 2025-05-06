// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.Repositories;
// using AppBlueprint.Presentation.ApiModule.Controllers.Baseline;
// using AppBlueprint.Infrastructure.DatabaseContexts.B2B.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Xunit;
// using TestContainers.Container.Abstractions.Hosting;
// using Testcontainers.PostgreSql;
// using Assert = TUnit.Assertions.Assert;
//
// public class SubscriptionControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     private readonly SubscriptionController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//     private readonly IConfiguration _configuration;
//
//     public SubscriptionControllerTests(IConfiguration configuration)
//     {
//         _configuration = configuration;
//         
//         _postgreSqlContainer = new PostgreSqlBuilder()
//             .WithDatabase("TestDb")
//             .WithUsername("postgres")
//             .WithPassword("yourStrong(!)Password")
//             .Build();
//
//         _dbContextOptions = new DbContextOptionsBuilder<ApplicationDBContext>()
//             .UseNpgsql(_postgreSqlContainer.GetConnectionString())
//             .Options;
//
//         var context = new ApplicationDBContext(_dbContextOptions, configuration: _configuration);
//         var subscriptionRepository = new SubscriptionRepository(context);
//         _controller = new SubscriptionController(subscriptionRepository);
//     }
//
//     [Fact]
//     public async Task GetSubscriptions_ReturnsOk_WhenSubscriptionsExist()
//     {
//         // Arrange
//         var subscriptions = new List<SubscriptionEntity>
//         {
//             new SubscriptionEntity { Id = 1, Name = "Subscription1" },
//             new SubscriptionEntity { Id = 2, Name = "Subscription2" }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions, configuration: _configuration))
//         {
//             context.Subscriptions.AddRange(subscriptions);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetSubscriptions(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert
//             .That().IsTypeOf(OkObjectResult)(result.Result);
//         var returnedSubscriptions = Assert.IsAssignableFrom<IEnumerable<SubscriptionEntity>>(okResult.Value);
//         Assert.Equal(2, returnedSubscriptions.Count());
//     }
//
//     [Fact]
//     public async Task GetSubscriptions_ReturnsNotFound_WhenNoSubscriptionsExist()
//     {
//         // Act
//         var result = await _controller.GetSubscriptions(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetSubscription_ReturnsOk_WhenSubscriptionExists()
//     {
//         // Arrange
//         var subscription = new SubscriptionEntity { Id = 1, Name = "Subscription1" };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Subscriptions.Add(subscription);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetSubscription(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedSubscription = Assert.IsType<SubscriptionEntity>(okResult.Value);
//         Assert.Equal("Subscription1", returnedSubscription.Name);
//     }
//
//     [Fact]
//     public async Task GetSubscription_ReturnsNotFound_WhenSubscriptionDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetSubscription(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



