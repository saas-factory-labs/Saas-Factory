using System.Linq;
using System.Threading.Tasks;
using AppBlueprint.Infrastructure.Services.PII;
using AppBlueprint.SharedKernel.Enums;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal class RegexPIIScannerTests
{
    private readonly RegexPIIScanner _scanner = new();

    [Test]
    public async Task ScanAsync_ShouldDetectEmail()
    {
        // Arrange
        const string text = "Contact me at mette@test.dk for info.";

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().Contain(t => t.Type == "Email" && t.Value == "mette@test.dk");
        tags.First(t => t.Type == "Email").Classification.Should().Be(GDPRType.DirectlyIdentifiable);
    }

    [Test]
    public async Task ScanAsync_ShouldDetectDanishPhone()
    {
        // Arrange
        const string text = "My number is +45 12 34 56 78.";

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().Contain(t => t.Type == "DanishPhone");
    }

    [Test]
    public async Task ScanAsync_ShouldDetectValidCreditCard_UsingLuhn()
    {
        // Arrange
        // A valid test VISA card number often used for testing is 4242 4242 4242 4242
        const string text = "Pay with card 4242-4242-4242-4242.";

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().Contain(t => t.Type == "CreditCard");
    }

    [Test]
    public async Task ScanAsync_ShouldNotDetectInvalidCreditCard_UsingLuhn()
    {
        // Arrange
        const string text = "Invalid card 1234-5678-9012-3456."; // Most likely fails Luhn

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().NotContain(t => t.Type == "CreditCard");
    }

    [Test]
    public async Task ScanAsync_ShouldDetectIPv4()
    {
        // Arrange
        const string text = "Server at 192.168.1.1 is down.";

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().Contain(t => t.Type == "IPv4" && t.Value == "192.168.1.1");
    }

    [Test]
    public async Task ScanAsync_ShouldDetectDanishCPR()
    {
        // Arrange
        const string text = "His CPR is 010190-1234.";

        // Act
        var results = await _scanner.ScanAsync(text);
        var tags = results.ToList();

        // Assert
        tags.Should().Contain(t => t.Type == "DanishCPR");
    }
}
