// public class DatabaseFixture : IDisposable
// {
//     // public DatabaseFixture()
//     // {
//     //     var options = new DbContextOptionsBuilder<B2BDbContext>()
//     //         .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Create a new in-memory database for each test
//     //         .Options;

//     //     Context = new B2BDbContext(options);
//     //     Context.Database.EnsureCreated();
//     // }

//     // public void Dispose()
//     // {
//     //     Context.Database.EnsureDeleted();
//     //     Context.Dispose();
//     // }
// }


/*

A DatabaseFixture is typically used in integration tests to set up a database environment for testing12. Here are some reasons why you might want to use a DatabaseFixture:

Consistent Test Environment: A DatabaseFixture can help ensure that your tests always run against a database with a known state1. This can make your tests more reliable and easier to reason about1.

Performance: By reusing the same DatabaseFixture across multiple tests, you can avoid the overhead of setting up and tearing down the database for each individual test1. This can significantly improve the performance of your test suite1.

Isolation: A DatabaseFixture can help isolate your tests from each other by providing each test with its own separate database context1. This can prevent tests from interfering with each other, making your test suite more robust1.

Realism: By testing against a real database (as opposed to an in-memory database or a mocked database), you can catch issues that might not show up when testing against a simplified or simulated database2.

*/



