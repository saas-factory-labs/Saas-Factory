using AppBlueprint.Infrastructure.Storage;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class FileValidationServiceTests
{
    [Test]
    [Arguments(".png", "image/png", true)]
    [Arguments(".jpg", "image/jpeg", true)]
    [Arguments(".jpeg", "image/jpeg", true)]
    [Arguments(".pdf", "application/pdf", true)]
    public async Task IsExtensionConsistentWithContentType_ShouldAcceptMatchingPairs(
        string extension, string contentType, bool expected)
    {
        bool result = FileValidationService.IsExtensionConsistentWithContentType(extension, contentType);

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    [Arguments(".png", "application/pdf")]
    [Arguments(".pdf", "image/png")]
    [Arguments(".jpg", "video/mp4")]
    [Arguments(".exe", "image/png")]
    public async Task IsExtensionConsistentWithContentType_ShouldRejectMismatchedPairs(
        string extension, string contentType)
    {
        bool result = FileValidationService.IsExtensionConsistentWithContentType(extension, contentType);

        result.Should().BeFalse(because: $"{extension} must not be paired with {contentType}");
        await Assert.That(result).IsFalse();
    }
}
