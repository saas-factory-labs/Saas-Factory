using System.Net;
using Aspire.Hosting;
using Projects;

namespace DeploymentManager.Tests;

internal sealed class WebTests
{
    [Test]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        IDistributedApplicationTestingBuilder? appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<DeploymentManager_AppHost>();
        await using DistributedApplication? app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act
        HttpClient? httpClient = app.CreateHttpClient("webfrontend");
        HttpResponseMessage? response = await httpClient.GetAsync(new Uri("/", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }
}
