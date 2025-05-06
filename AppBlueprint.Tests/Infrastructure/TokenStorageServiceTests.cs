using System.Globalization;
using AppBlueprint.Infrastructure.Authorization;
using Microsoft.JSInterop;
using NSubstitute;
using Xunit;

namespace AppBlueprint.Tests.Infrastructure;

public class TokenStorageServiceTests
{
    private readonly IJSRuntime _jsRuntimeMock;
    private readonly TokenStorageService _tokenStorageService;

    public TokenStorageServiceTests()
    {
        _jsRuntimeMock = Substitute.For<IJSRuntime>();
        _tokenStorageService = new TokenStorageService(_jsRuntimeMock);
    }

    [Fact]
    public async Task SaveTokenAsync_ShouldStoreTokenAndExpiration()
    {
        // Arrange
        string testToken = "test_token";
        DateTime expiration = DateTime.UtcNow.AddHours(1);

        // Act
        await _tokenStorageService.SaveTokenAsync(testToken, expiration);

        // Assert
        await _jsRuntimeMock.Received(1).InvokeVoidAsync("localStorage.setItem", "auth_token", testToken);
        await _jsRuntimeMock.Received(1).InvokeVoidAsync("localStorage.setItem", "auth_expiration", expiration.ToString("O"));
    }

    [Fact]
    public async Task GetTokenAsync_ShouldRetrieveStoredToken()
    {
        // Arrange
        string expectedToken = "test_token";
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_token")
            .Returns(ValueTask.FromResult<string?>(expectedToken));

        // Act
        var token = await _tokenStorageService.GetTokenAsync();

        // Assert
        Assert.Equal(expectedToken, token);
        await _jsRuntimeMock.Received(1).InvokeAsync<string?>("localStorage.getItem", "auth_token");
    }

    [Fact]
    public async Task GetTokenExpirationAsync_ShouldRetrieveStoredExpiration()
    {
        // Arrange
        DateTime expectedExpiration = DateTime.UtcNow.AddHours(1);
        string expirationString = expectedExpiration.ToString("O", CultureInfo.InvariantCulture);
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_expiration")
            .Returns(ValueTask.FromResult<string?>(expirationString));

        // Act
        var expiration = await _tokenStorageService.GetTokenExpirationAsync();

        // Assert
        Assert.NotNull(expiration);
        Assert.Equal(expectedExpiration, expiration);
        await _jsRuntimeMock.Received(1).InvokeAsync<string?>("localStorage.getItem", "auth_expiration");
    }

    [Fact]
    public async Task ClearTokenAsync_ShouldRemoveTokenAndExpiration()
    {
        // Act
        await _tokenStorageService.ClearTokenAsync();

        // Assert
        await _jsRuntimeMock.Received(1).InvokeVoidAsync("localStorage.removeItem", "auth_token");
        await _jsRuntimeMock.Received(1).InvokeVoidAsync("localStorage.removeItem", "auth_expiration");
    }

    [Fact]
    public async Task IsTokenValidAsync_ShouldReturnTrueForValidToken()
    {
        // Arrange
        string testToken = "test_token";
        DateTime expiration = DateTime.UtcNow.AddHours(1);
        string expirationString = expiration.ToString("O", CultureInfo.InvariantCulture);
        
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_token")
            .Returns(ValueTask.FromResult<string?>(testToken));
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_expiration")
            .Returns(ValueTask.FromResult<string?>(expirationString));

        // Act
        var isValid = await _tokenStorageService.IsTokenValidAsync();

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task IsTokenValidAsync_ShouldReturnFalseForExpiredToken()
    {
        // Arrange
        string testToken = "test_token";
        DateTime expiration = DateTime.UtcNow.AddHours(-1); // Expired token
        string expirationString = expiration.ToString("O", CultureInfo.InvariantCulture);
        
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_token")
            .Returns(ValueTask.FromResult<string?>(testToken));
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_expiration")
            .Returns(ValueTask.FromResult<string?>(expirationString));

        // Act
        var isValid = await _tokenStorageService.IsTokenValidAsync();

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public async Task IsTokenValidAsync_ShouldReturnFalseForMissingToken()
    {
        // Arrange
        _jsRuntimeMock.InvokeAsync<string?>("localStorage.getItem", "auth_token")
            .Returns(ValueTask.FromResult<string?>(null));

        // Act
        var isValid = await _tokenStorageService.IsTokenValidAsync();

        // Assert
        Assert.False(isValid);
    }
}