using AppBlueprint.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using TUnit.Assertions;
using TUnit.Core;

namespace AppBlueprint.Tests.SignalR;

/// <summary>
/// Unit tests for NotificationHub to verify real-time notification functionality and tenant isolation.
/// </summary>
public sealed class NotificationHubTests
{
    private readonly Mock<ILogger<NotificationHub>> _loggerMock;
    private readonly Mock<HubCallerContext> _contextMock;
    private readonly Mock<IGroupManager> _groupManagerMock;
    private readonly Mock<IHubCallerClients> _clientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly NotificationHub _hub;

    public NotificationHubTests()
    {
        _loggerMock = new Mock<ILogger<NotificationHub>>();
        _contextMock = new Mock<HubCallerContext>();
        _groupManagerMock = new Mock<IGroupManager>();
        _clientsMock = new Mock<IHubCallerClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _hub = new NotificationHub(_loggerMock.Object)
        {
            Context = _contextMock.Object,
            Groups = _groupManagerMock.Object,
            Clients = _clientsMock.Object
        };
    }

    [Test]
    public async Task OnConnectedAsync_WithTenantId_AddsConnectionToTenantGroup()
    {
        // Arrange
        string tenantId = "tenant-123";
        string connectionId = "connection-456";
        
        var claims = new List<Claim> { new Claim("tenant_id", tenantId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);
        _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);
        _groupManagerMock
            .Setup(g => g.AddToGroupAsync(connectionId, $"tenant-{tenantId}", default))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groupManagerMock.Verify(
            g => g.AddToGroupAsync(connectionId, $"tenant-{tenantId}", default),
            Times.Once);
    }

    [Test]
    public async Task OnConnectedAsync_WithoutTenantId_DoesNotAddToGroup()
    {
        // Arrange
        string connectionId = "connection-456";
        
        var claims = new List<Claim>(); // No tenant_id claim
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);
        _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groupManagerMock.Verify(
            g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), default),
            Times.Never);
    }

    [Test]
    public async Task OnDisconnectedAsync_WithException_LogsError()
    {
        // Arrange
        string connectionId = "connection-456";
        var exception = new Exception("Test exception");
        
        _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

        // Act
        await _hub.OnDisconnectedAsync(exception);

        // Assert - Verify error was logged (check that LogError was called)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task OnDisconnectedAsync_WithoutException_LogsInformation()
    {
        // Arrange
        string connectionId = "connection-456";
        
        _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

        // Act
        await _hub.OnDisconnectedAsync(null);

        // Assert - Verify information was logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Test]
    public async Task SendNotificationToTenant_WithTenantId_SendsToTenantGroup()
    {
        // Arrange
        string tenantId = "tenant-123";
        string message = "Test notification message";
        
        var claims = new List<Claim> { new Claim("tenant_id", tenantId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);
        _clientsMock.Setup(c => c.Group($"tenant-{tenantId}")).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendNotificationToTenant(message);

        // Assert
        _clientProxyMock.Verify(
            cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default),
            Times.Once);
    }

    [Test]
    public async Task SendNotificationToTenant_WithoutTenantId_ThrowsHubException()
    {
        // Arrange
        string message = "Test notification message";
        
        var claims = new List<Claim>(); // No tenant_id claim
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);

        // Act & Assert
        await Assert.ThrowsAsync<HubException>(async () => 
            await _hub.SendNotificationToTenant(message));
    }

    [Test]
    public async Task SendNotificationToTenant_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        string? message = null;
        
        var claims = new List<Claim> { new Claim("tenant_id", "tenant-123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await _hub.SendNotificationToTenant(message!));
    }

    [Test]
    public async Task SendNotificationToUser_WithTenantId_SendsToSpecificUser()
    {
        // Arrange
        string tenantId = "tenant-123";
        string userId = "user-456";
        string message = "Test user notification";
        
        var claims = new List<Claim> { new Claim("tenant_id", tenantId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);
        _clientsMock.Setup(c => c.User(userId)).Returns(_clientProxyMock.Object);
        _clientProxyMock
            .Setup(cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default))
            .Returns(Task.CompletedTask);

        // Act
        await _hub.SendNotificationToUser(userId, message);

        // Assert
        _clientProxyMock.Verify(
            cp => cp.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(args => args.Length == 1 && (string)args[0] == message),
                default),
            Times.Once);
    }

    [Test]
    public async Task SendNotificationToUser_WithoutTenantId_ThrowsHubException()
    {
        // Arrange
        string userId = "user-456";
        string message = "Test user notification";
        
        var claims = new List<Claim>(); // No tenant_id claim
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);

        // Act & Assert
        await Assert.ThrowsAsync<HubException>(async () => 
            await _hub.SendNotificationToUser(userId, message));
    }

    [Test]
    public async Task SendNotificationToUser_WithNullUserId_ThrowsArgumentNullException()
    {
        // Arrange
        string? userId = null;
        string message = "Test user notification";
        
        var claims = new List<Claim> { new Claim("tenant_id", "tenant-123") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _contextMock.Setup(c => c.User).Returns(principal);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            await _hub.SendNotificationToUser(userId!, message));
    }
}
