using System.Net;
using AppBlueprint.Application.Validations;
using FluentAssertions;

namespace AppBlueprint.Tests.Application;

internal sealed class WebhookUrlValidatorTests
{
    [Test]
    [Arguments("ftp://example.com/hook")]
    [Arguments("file:///etc/passwd")]
    [Arguments("javascript:alert(1)")]
    [Arguments("gopher://example.com")]
    public async Task TryValidate_ShouldRejectNonHttpSchemes(string url)
    {
        bool ok = WebhookUrlValidator.TryValidate(ParseUri(url), out string? error);

        ok.Should().BeFalse();
        await Assert.That(error).IsNotNull();
    }

    [Test]
    [Arguments("http://127.0.0.1/hook")]
    [Arguments("http://localhost/hook")]
    [Arguments("http://169.254.169.254/latest/meta-data")]
    [Arguments("http://10.0.0.5/hook")]
    [Arguments("http://192.168.1.10/hook")]
    [Arguments("http://172.16.5.4/hook")]
    public async Task TryValidate_ShouldRejectInternalAndMetadataTargets(string url)
    {
        bool ok = WebhookUrlValidator.TryValidate(ParseUri(url), out string? error);

        ok.Should().BeFalse(because: $"{url} targets an internal or metadata address");
        await Assert.That(error).IsNotNull();
    }

    [Test]
    [Arguments("not-a-url")]
    [Arguments("")]
    [Arguments("//relative/path")]
    public async Task TryValidate_ShouldRejectMalformedUrls(string url)
    {
        bool ok = WebhookUrlValidator.TryValidate(ParseUri(url), out _);

        await Assert.That(ok).IsFalse();
    }

    [Test]
    [Arguments("https://93.184.216.34/hook")]
    [Arguments("https://8.8.8.8/callback")]
    public async Task TryValidate_ShouldAcceptPublicHttpsTargets(string url)
    {
        bool ok = WebhookUrlValidator.TryValidate(ParseUri(url), out string? error);

        ok.Should().BeTrue(because: error);
        await Assert.That(error).IsNull();
    }

    [Test]
    [Arguments("127.0.0.1", true)]
    [Arguments("10.1.2.3", true)]
    [Arguments("172.31.255.255", true)]
    [Arguments("192.168.0.1", true)]
    [Arguments("169.254.169.254", true)]
    [Arguments("8.8.8.8", false)]
    [Arguments("93.184.216.34", false)]
    [Arguments("::1", true)]
    public async Task IsBlockedIpAddress_ShouldClassifyRanges(string ip, bool expectedBlocked)
    {
        bool blocked = WebhookUrlValidator.IsBlockedIpAddress(IPAddress.Parse(ip));

        await Assert.That(blocked).IsEqualTo(expectedBlocked);
    }

    private static Uri? ParseUri(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri? absoluteUri))
        {
            return absoluteUri;
        }

        if (Uri.TryCreate(url, UriKind.Relative, out Uri? relativeUri))
        {
            return relativeUri;
        }

        return null;
    }
}
