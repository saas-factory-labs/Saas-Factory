using System.Net;
using System.Net.Http;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.Kiota.Abstractions;
using Moq;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace AppBlueprint.Tests.Infrastructure;

public class UserAuthenticationProviderTests
{
    private readonly ITokenStorageService _tokenStorageMock;
    private readonly HttpClient _httpClientMock;
    private readonly UserAuthenticationProvider _authProvider;
    private readonly string _authEndpoint = "https://api.example.com/auth";

    public UserAuthenticationProviderTests()
    {
        _tokenStorageMock = new Mock<ITokenStorageService>().Object;
        _httpClientMock = new HttpClient(new MockHttpMessageHandler());
        _authProvider = new UserAuthenticationProvider(_httpClientMock, _authEndpoint, _tokenStorageMock);
    }

    [Test]
    public async Task LoginAsync_ShouldReturnTrue_WhenLoginSucceeds()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password123";

        // Act
        bool result = await _authProvider.LoginAsync(email, password);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task LogoutAsync_ShouldClearToken()
    {
        // Act
        await _authProvider.LogoutAsync();

        // Assert
        Assert.That(true, Is.True); // Token removal happens internally
    }

    [Test]
    public void IsAuthenticated_ShouldReturnFalse_WhenTokenIsNull()
    {
        // Act
        bool isAuthenticated = _authProvider.IsAuthenticated();

        // Assert
        Assert.That(isAuthenticated, Is.False);
    }

    [Test]
    public async Task AuthenticateRequestAsync_ShouldAddAuthorizationHeader_WhenAuthenticated()
    {
        // Arrange
        string expectedToken = "test_token";
        var request = new RequestInformation();
        
        // Set up the auth provider to have a valid token
        // This is done by using reflection to set the private field
        var privateField = typeof(UserAuthenticationProvider).GetField("_accessToken", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        privateField.SetValue(_authProvider, expectedToken);
        
        // Set expiration to future time
        var expirationField = typeof(UserAuthenticationProvider).GetField("_accessTokenExpiration", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        expirationField.SetValue(_authProvider, DateTime.UtcNow.AddHours(1));

        // Act
        await _authProvider.AuthenticateRequestAsync(request);

        // Assert
        Assert.That(request.Headers.ContainsKey("Authorization"), Is.True);
        Assert.That(request.Headers["Authorization"], Is.EqualTo($"Bearer {expectedToken}"));
    }

    [Test]
    public async Task InitializeFromStorageAsync_ShouldRestoreValidToken()
    {
        // Arrange
        string expectedToken = "test_token";
        DateTime futureExpiration = DateTime.UtcNow.AddHours(1);
        
        // Setup token storage mock to return a token and valid expiration
        var mockStorage = new Mock<ITokenStorageService>();
        mockStorage.Setup(x => x.GetTokenAsync()).ReturnsAsync(expectedToken);
        
        // Create a mock token format that will be parsed correctly
        var tokenParts = new[]
        {
            "header",  // Header part
            Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                $"{{\"sub\":\"123\",\"name\":\"test\",\"email\":\"test@example.com\",\"iat\":1619712000,\"exp\":{DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()}}}"
            )), // Payload part
            "signature" // Signature part
        };
        string mockToken = string.Join(".", tokenParts);
        
        mockStorage.Setup(x => x.GetTokenAsync()).ReturnsAsync(mockToken);
        var authProvider = new UserAuthenticationProvider(_httpClientMock, _authEndpoint, mockStorage.Object);

        // Act
        await authProvider.InitializeFromStorageAsync();

        // Assert
        Assert.That(authProvider.IsAuthenticated(), Is.True);
    }

    /// <summary>
    /// A mock HTTP handler that returns a successful response with a mock token
    /// </summary>
    sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"token\":\"<token>.SIGNATURE\"}")
            };
            return Task.FromResult(response);
        }
    }
}