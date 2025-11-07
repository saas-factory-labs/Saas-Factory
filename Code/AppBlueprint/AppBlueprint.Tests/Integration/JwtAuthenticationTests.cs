using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace AppBlueprint.Tests.Integration;

/// <summary>
/// Integration tests for JWT authentication
/// </summary>
[Category("JwtAuth")]
public class JwtAuthenticationTests
{
    private const string SecretKey = "YourSuperSecretKey_ChangeThisInProduction_MustBeAtLeast32Characters!";
    private const string Issuer = "AppBlueprintAPI";
    private const string Audience = "AppBlueprintClient";

    private static string GenerateJwtToken(string userId, string userName, string email, string[] roles, int expirationHours = 1)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [Test]
    public async Task PublicEndpoint_WithoutToken_ShouldReturn200()
    {
        // Arrange
        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };

        // Act
        var response = await client.GetAsync("/api/v1/authtest/public");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task ProtectedEndpoint_WithoutToken_ShouldReturn401()
    {
        // Arrange
        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };

        // Act
        var response = await client.GetAsync("/api/v1/authtest/protected");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task ProtectedEndpoint_WithValidToken_ShouldReturn200()
    {
        // Arrange
        var token = GenerateJwtToken(
            "test-user-123",
            "Test User",
            "test@example.com",
            new[] { "User" });

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/protected");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("Successfully authenticated");
    }

    [Test]
    public async Task ProtectedEndpoint_WithExpiredToken_ShouldReturn401()
    {
        // Arrange - Generate token that expired 1 hour ago
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Name, "Test User")
            }),
            Expires = DateTime.UtcNow.AddHours(-1), // Expired!
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var expiredToken = tokenHandler.WriteToken(token);

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/protected");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task AdminEndpoint_WithUserRole_ShouldReturn403()
    {
        // Arrange
        var token = GenerateJwtToken(
            "test-user-123",
            "Test User",
            "test@example.com",
            new[] { "User" }); // User role, not Admin

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/admin");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task AdminEndpoint_WithAdminRole_ShouldReturn200()
    {
        // Arrange
        var token = GenerateJwtToken(
            "admin-user-123",
            "Admin User",
            "admin@example.com",
            new[] { "Admin" }); // Admin role

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/admin");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task UserEndpoint_WithUserRole_ShouldReturn200()
    {
        // Arrange
        var token = GenerateJwtToken(
            "test-user-123",
            "Test User",
            "test@example.com",
            new[] { "User" });

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/user");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
    }

    [Test]
    public async Task ProtectedEndpoint_WithInvalidSignature_ShouldReturn401()
    {
        // Arrange - Generate token with wrong secret key
        var tokenHandler = new JwtSecurityTokenHandler();
        var wrongKey = Encoding.ASCII.GetBytes("WrongSecretKey_ThisWillNotWork_AtLeast32Characters!");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Name, "Test User")
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(wrongKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var invalidToken = tokenHandler.WriteToken(token);

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/protected");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task EchoEndpoint_ReturnsUserClaims()
    {
        // Arrange
        var token = GenerateJwtToken(
            "test-user-123",
            "Test User",
            "test@example.com",
            new[] { "User", "Premium" });

        using var client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5002")
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/v1/authtest/echo");

        // Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        await Assert.That(content).Contains("Test User");
        await Assert.That(content).Contains("test@example.com");
    }
}

