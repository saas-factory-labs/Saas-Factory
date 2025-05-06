// using System.Net;
// using Aspire.Hosting;
// using Assert = TUnit.Assertions.Assert;
//
// namespace AppBlueprint.Tests;
//
// public class WebTests
// {
//     [Test]
//     public async Task GetWebResourceRootReturnsOkStatusCode()
//     {
//         // Arrange
//         IDistributedApplicationTestingBuilder? appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppBlueprint_AppHost>();
//         await using DistributedApplication? app = await appHost.BuildAsync();
//         await app.StartAsync();
//
//         // Act
//         HttpClient? httpClient = app.CreateHttpClient("webfrontend");
//         HttpResponseMessage? response = await httpClient.GetAsync("/");
//
//         // Assert
//         await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
//     }
// }



