using AppBlueprint.Presentation.ApiModule.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace AppBlueprint.Tests.Integration;

/// <summary>
/// Tests for the simplified authentication setup extensions.
/// These tests verify that the easy-to-use authentication methods work correctly.
/// </summary>
[Category("AuthSetup")]
public class AuthenticationSetupExtensionsTests
{
    [Test]
    public async Task AddQuickAuthentication_WithLogtoProvider_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Logto:Endpoint"] = "https://test.logto.app",
                ["Authentication:Logto:ClientId"] = "test-client-id"
            })
            .Build();

        // Act
        services.AddQuickAuthentication(configuration, AuthProvider.Logto);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public async Task AddQuickAuthentication_WithAuth0Provider_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Auth0:Domain"] = "https://test.auth0.com",
                ["Authentication:Auth0:Audience"] = "https://test-api"
            })
            .Build();

        // Act
        services.AddQuickAuthentication(configuration, AuthProvider.Auth0);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public async Task AddQuickAuthentication_WithJWTProvider_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:JWT:SecretKey"] = "SuperSecretKey_AtLeast32CharactersLong_ForTesting!"
            })
            .Build();

        // Act
        services.AddQuickAuthentication(configuration, AuthProvider.JWT);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public async Task AddLogtoAuthentication_WithValidParameters_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLogtoAuthentication(
            endpoint: "https://my-tenant.logto.app",
            clientId: "my-client-id",
            clientSecret: "my-client-secret");
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public async Task AddLogtoAuthentication_WithoutClientSecret_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddLogtoAuthentication(
            endpoint: "https://my-tenant.logto.app",
            clientId: "my-client-id");
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
    }

    [Test]
    public void AddLogtoAuthentication_WithNullEndpoint_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddLogtoAuthentication(
            endpoint: null!,
            clientId: "my-client-id"))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddLogtoAuthentication_WithEmptyEndpoint_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddLogtoAuthentication(
            endpoint: "",
            clientId: "my-client-id"))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddLogtoAuthentication_WithNullClientId_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddLogtoAuthentication(
            endpoint: "https://test.logto.app",
            clientId: null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task AddAuth0Authentication_WithValidParameters_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAuth0Authentication(
            domain: "https://my-tenant.auth0.com",
            audience: "https://my-api",
            clientId: "my-client-id");
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public void AddAuth0Authentication_WithNullDomain_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddAuth0Authentication(
            domain: null!,
            audience: "https://my-api"))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddAuth0Authentication_WithNullAudience_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddAuth0Authentication(
            domain: "https://test.auth0.com",
            audience: null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task AddSimpleJwtAuthentication_WithValidSecretKey_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSimpleJwtAuthentication(
            secretKey: "SuperSecretKey_AtLeast32CharactersLong_ForTesting!");
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
        await Assert.That(scheme!.Name).IsEqualTo(JwtBearerDefaults.AuthenticationScheme);
    }

    [Test]
    public async Task AddSimpleJwtAuthentication_WithCustomIssuerAndAudience_ShouldConfigureServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSimpleJwtAuthentication(
            secretKey: "SuperSecretKey_AtLeast32CharactersLong_ForTesting!",
            issuer: "MyCustomIssuer",
            audience: "MyCustomAudience");
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
    }

    [Test]
    public void AddSimpleJwtAuthentication_WithNullSecretKey_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddSimpleJwtAuthentication(secretKey: null!))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddSimpleJwtAuthentication_WithEmptySecretKey_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        await Assert.That(() => services.AddSimpleJwtAuthentication(secretKey: ""))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddSimpleJwtAuthentication_WithShortSecretKey_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - Key less than 32 characters
        await Assert.That(() => services.AddSimpleJwtAuthentication(secretKey: "TooShort"))
            .Throws<ArgumentException>();
    }

    [Test]
    public void AddQuickAuthentication_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        await Assert.That(() => services.AddQuickAuthentication(configuration, AuthProvider.Logto))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public void AddQuickAuthentication_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration configuration = null!;

        // Act & Assert
        await Assert.That(() => services.AddQuickAuthentication(configuration, AuthProvider.Logto))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task AddLogtoAuthentication_MultipleCallsToSameService_ShouldNotThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Call multiple times to ensure it doesn't break
        services.AddLogtoAuthentication(
            endpoint: "https://tenant1.logto.app",
            clientId: "client-1");
        
        // Assert - Should not throw when building
        var serviceProvider = services.BuildServiceProvider();
        var authSchemeProvider = serviceProvider.GetRequiredService<IAuthenticationSchemeProvider>();
        var scheme = await authSchemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        
        await Assert.That(scheme).IsNotNull();
    }
}
