using AppBlueprint.Application.Interfaces;
using AppBlueprint.Infrastructure.Hubs;
using AppBlueprint.Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using TUnit.Assertions;
using TUnit.Core;

namespace AppBlueprint.Tests.SignalR;

/// <summary>
/// Unit tests for SignalRNotificationService to verify notification delivery to tenants and users.
/// </summary>
public sealed class SignalRNotificationServiceTests
{
    private readonly Mock<IHubContext<NotificationHub>> _hubContextMock;
    private readonly Mock<ILogger<SignalRNotificationService>> _loggerMock;
    private readonly Mock<IHubClients> _clientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly INotificationService _service;

    public SignalRNotificationServiceTests()
    {
        _hubContextMock = new Mock<IHubContext<NotificationHub>>();
        _loggerMock = new Mock<ILogger<SignalRNotificationService>>();
        _clientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _hubContextMock.Setup(h => h.Clients).Returns(_clientsMock.Object);

        _service = new SignalRNotificationService(
            _hubContextMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public async Task SendToTenantAsync_WithValidTenantId_SendsNotificationToTenantGroup()
    {
        // Arrange
        string tenantId = "tenant-123";
        string message = "Test notification for tenant";

        _clientsMock.Setup(c => c.Group($"tenant-{tenantId}")).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToTenantAsync(tenantId, message);

        // Assert
        _clientProxyMock.Verify(
            cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default),
            Times.Once);
    }

    [Test]
    public async Task SendToTenantAsync_WithNullTenantId_ThrowsArgumentNullException()
    {
        // Arrange
        string? tenantId = null;
        string message = "Test notification";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.SendToTenantAsync(tenantId!, message));
    }

    [Test]
    public async Task SendToTenantAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string tenantId = "tenant-123";
        string? message = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.SendToTenantAsync(tenantId, message!));
    }

    [Test]
    public async Task SendToUserAsync_WithValidUserId_SendsNotificationToUser()
    {
        // Arrange
        string userId = "user-456";
        string message = "Test notification for user";

        _clientsMock.Setup(c => c.User(userId)).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToUserAsync(userId, message);

        // Assert
        _clientProxyMock.Verify(
            cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default),
            Times.Once);
    }

    [Test]
    public async Task SendToUserAsync_WithNullUserId_ThrowsArgumentNullException()
    {
        // Arrange
        string? userId = null;
        string message = "Test notification";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.SendToUserAsync(userId!, message));
    }

    [Test]
    public async Task SendToUserAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string userId = "user-456";
        string? message = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.SendToUserAsync(userId, message!));
    }

    [Test]
    public async Task SendToAllAsync_WithValidMessage_SendsNotificationToAllClients()
    {
        // Arrange
        string message = "Broadcast notification to all clients";

        _clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToAllAsync(message);

        // Assert
        _clientProxyMock.Verify(
            cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default),
            Times.Once);
    }

    [Test]
    public async Task SendToAllAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string? message = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.SendToAllAsync(message!));
    }

    [Test]
    public async Task SendToTenantAsync_LogsInformation()
    {
        // Arrange
        string tenantId = "tenant-123";
        string message = "Test notification for logging";

        _clientsMock.Setup(c => c.Group($"tenant-{tenantId}")).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.IsAny<object[]>(),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToTenantAsync(tenantId, message);

        // Assert - Verify information logging occurred
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task SendToUserAsync_LogsInformation()
    {
        // Arrange
        string userId = "user-456";
        string message = "Test notification for logging";

        _clientsMock.Setup(c => c.User(userId)).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.IsAny<object[]>(),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToUserAsync(userId, message);

        // Assert - Verify information logging occurred
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task SendToAllAsync_LogsWarning()
    {
        // Arrange
        string message = "Broadcast notification";

        _clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.IsAny<object[]>(),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SendToAllAsync(message);

        // Assert - Verify warning logging occurred (because it bypasses tenant isolation)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
