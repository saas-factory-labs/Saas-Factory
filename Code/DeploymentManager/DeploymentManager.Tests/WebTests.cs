using System.Net;
using Aspire.Hosting;
using Projects;

namespace DeploymentManager.Tests;

public class WebTests
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        IDistributedApplicationTestingBuilder? appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<DeploymentManager_AppHost>();
        await using DistributedApplication? app = await appHost.BuildAsync();
        await app.StartAsync();

        // Act
        HttpClient? httpClient = app.CreateHttpClient("webfrontend");
        HttpResponseMessage? response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
