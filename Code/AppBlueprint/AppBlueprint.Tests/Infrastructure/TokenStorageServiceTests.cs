using AppBlueprint.Infrastructure.Authorization;
using FluentAssertions;
using Microsoft.JSInterop;
using Moq;
using TUnit;

namespace AppBlueprint.Tests.Infrastructure
{
    public class TokenStorageServiceTests
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly TokenStorageService _tokenStorageService;
        private const string TestToken = "test_token_value";
        private const string StorageKey = "auth_token";

        public TokenStorageServiceTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _tokenStorageService = new TokenStorageService(_jsRuntimeMock.Object);
        }

        [Test]
        public async Task StoreTokenAsync_ShouldStoreTokenInLocalStorage()
        {
            // Arrange
            _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.setItem", StorageKey, TestToken))
                .Returns(ValueTask.CompletedTask);

            // Act
            await _tokenStorageService.StoreTokenAsync(TestToken);

            // Assert
            _jsRuntimeMock.Verify(js => js.InvokeVoidAsync("localStorage.setItem", StorageKey, TestToken), Times.Once);
        }

        [Test]
        public async Task GetTokenAsync_ShouldRetrieveTokenFromLocalStorage()
        {
            // Arrange
            _jsRuntimeMock.Setup(js => js.InvokeAsync<string?>("localStorage.getItem", StorageKey))
                .ReturnsAsync(TestToken);

            // Act
            var result = await _tokenStorageService.GetTokenAsync();

            // Assert
            result.Should().Be(TestToken);
            _jsRuntimeMock.Verify(js => js.InvokeAsync<string?>("localStorage.getItem", StorageKey), Times.Once);
        }

        [Test]
        public async Task RemoveTokenAsync_ShouldRemoveTokenFromLocalStorage()
        {
            // Arrange
            _jsRuntimeMock.Setup(js => js.InvokeVoidAsync("localStorage.removeItem", StorageKey))
                .Returns(ValueTask.CompletedTask);

            // Act
            await _tokenStorageService.RemoveTokenAsync();

            // Assert
            _jsRuntimeMock.Verify(js => js.InvokeVoidAsync("localStorage.removeItem", StorageKey), Times.Once);
        }
    }
}