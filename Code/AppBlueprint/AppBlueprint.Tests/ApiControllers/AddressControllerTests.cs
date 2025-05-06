// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using AppBlueprint.Infrastructure.DatabaseContexts.Baseline.Entities;
// using AppBlueprint.Infrastructure.Repositories.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Xunit;
// using Testcontainers.Container.Abstractions.Hosting;
// using Testcontainers.Container.Database.PostgreSql;
//
// public class AddressControllerTests
// {
//     private readonly PostgreSqlContainer _postgreSqlContainer;
//     // private readonly AddressController _controller;
//     private readonly DbContextOptions<ApplicationDBContext> _dbContextOptions;
//
//     public AddressControllerTests()
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
//         var addressRepository = new AddressRepository(context);
//         _controller = new AddressController(addressRepository);
//     }
//
//     [Fact]
//     public async Task GetAddresses_ReturnsOk_WhenAddressesExist()
//     {
//         // Arrange
//         var addresses = new List<AddressEntity>
//         {
//             new AddressEntity { Id = 1, StreetNumber = "123" },
//             new AddressEntity { Id = 2, StreetNumber = "456" }
//         };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Addresses.AddRange(addresses);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetAddresses(CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedAddresses = Assert.IsAssignableFrom<IEnumerable<AddressEntity>>(okResult.Value);
//         Assert.Equal(2, returnedAddresses.Count());
//     }
//
//     [Fact]
//     public async Task GetAddresses_ReturnsNotFound_WhenNoAddressesExist()
//     {
//         // Act
//         var result = await _controller.GetAddresses(CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
//
//     [Fact]
//     public async Task GetAddress_ReturnsOk_WhenAddressExists()
//     {
//         // Arrange
//         var address = new AddressEntity { Id = 1, StreetNumber = "123" };
//
//         using (var context = new ApplicationDBContext(_dbContextOptions))
//         {
//             context.Addresses.Add(address);
//             await context.SaveChangesAsync();
//         }
//
//         // Act
//         var result = await _controller.GetAddress(1, CancellationToken.None);
//
//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result.Result);
//         var returnedAddress = Assert.IsType<AddressEntity>(okResult.Value);
//         Assert.Equal("123", returnedAddress.StreetNumber);
//     }
//
//     [Fact]
//     public async Task GetAddress_ReturnsNotFound_WhenAddressDoesNotExist()
//     {
//         // Act
//         var result = await _controller.GetAddress(1, CancellationToken.None);
//
//         // Assert
//         Assert.IsType<NotFoundObjectResult>(result.Result);
//     }
// }



