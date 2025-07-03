using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
// using AppBlueprint.Tests.Infrastructure; // Removed duplicate
using AppBlueprint.Tests.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AppBlueprint.Tests;


internal sealed class AuthorizationTests : IDisposable
{
    internal const string AdminToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
    internal const string UserToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
    internal const string TenantAToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
    private bool _disposed;
    private WebApplicationFactory<TestStartup>? _factory;
    private HttpClient? _http;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void InitializeHttpClient()
    {
        if (_factory != null)
        {
            _http?.Dispose();
            _factory.Dispose();
        }

        // Create a test factory for the API service
        _factory = new WebApplicationFactory<TestStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestAuth", options => { });
                });
            });

        _http = _factory.CreateClient();
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _http?.Dispose();
            _http = null;

            _factory?.Dispose();
            _factory = null;
        }

        _disposed = true;
    }

    ~AuthorizationTests()
    {
        Dispose(false);
    }

    [Test]
    public async Task ShouldReturn401WhenNotAuthenticated()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization = null;

        // Act
        HttpResponseMessage response = await _http.GetAsync(new Uri("/api/admin/secret", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
        await Assert.That(await response.Content.ReadAsStringAsync()).Contains("Authorization");
    }

    [Test]
    public async Task ShouldReturn200WhenUserIsAdmin()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminToken);

        // Act
        HttpResponseMessage response = await _http.GetAsync(new Uri("/api/admin/secret", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        string content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).IsNotEmpty();
    }

    [Test]
    public async Task ShouldReturn403WhenUserNotAdmin()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", UserToken);

        // Act
        HttpResponseMessage response = await _http.GetAsync(new Uri("/api/admin/secret", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        string content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("Forbidden");
    }

    [Test]
    public async Task ShouldNotAllowAccessToOtherTenantsResource()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TenantAToken);

        // Act
        HttpResponseMessage response =
            await _http.GetAsync(new Uri("/api/tenants/other-tenant/resource", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        string content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("tenant");
    }

    [Test]
    public async Task ShouldRejectXssInUserInput()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminToken);
        object payload = new { Name = "<script>alert('xss')</script>" };

        // Act
        HttpResponseMessage response = await _http.PostAsJsonAsync(new Uri("/api/users", UriKind.Relative), payload);

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
        string content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("invalid");
    }

    [Test]
    public async Task ShouldNotReturnDeletedUser()
    {
        // Arrange
        InitializeHttpClient();
        _http!.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AdminToken);

        // Act
        HttpResponseMessage response = await _http.GetAsync(new Uri("/api/users/deleted-user-id", UriKind.Relative));

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
        string content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("not found");
    }

    [Test]
    public void ShouldRequireAuthorizationOnAllControllers()
    {
        // Arrange
        Type[] controllerTypes = typeof(TestStartup).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && !t.IsAbstract)
            .ToArray();

        // Act & Assert
        foreach (Type controller in controllerTypes)
        {
            bool hasAuthorize =
                controller.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0 ||
                controller.GetMethods().Any(m => m.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length > 0) ||
                controller.GetMethods()
                    .All(m => m.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0);

            if (!hasAuthorize)
                throw new InvalidOperationException(
                    $"Controller {controller.Name} is missing authorization attributes.");
        }
    }
}
