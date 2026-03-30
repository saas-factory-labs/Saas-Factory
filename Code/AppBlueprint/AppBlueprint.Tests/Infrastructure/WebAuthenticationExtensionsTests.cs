using AppBlueprint.Infrastructure.Authentication;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class WebAuthenticationExtensionsTests
{
    [Test]
    [Arguments("https://32nkyp.logto.app/oidc", "https://32nkyp.logto.app/oidc")]
    [Arguments("https://32nkyp.logto.app", "https://32nkyp.logto.app/oidc")]
    [Arguments("https://32nkyp.logto.app/", "https://32nkyp.logto.app/oidc")]
    [Arguments("https://32nkyp.logto.app/oidc/", "https://32nkyp.logto.app/oidc")]
    [Arguments("https://32nkyp.logto.app/OIDC", "https://32nkyp.logto.app/OIDC")]
    public async Task NormalizeLogtoOidcEndpoint_ShouldNeverDoubleAppendOidc(string input, string expected)
    {
        var result = WebAuthenticationExtensions.NormalizeLogtoOidcEndpoint(input);

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    public async Task NormalizeLogtoOidcEndpoint_ShouldReturnNullOrEmptyUnchanged(string? input)
    {
        var result = WebAuthenticationExtensions.NormalizeLogtoOidcEndpoint(input);

        await Assert.That(result).IsEqualTo(input);
    }

    [Test]
    public async Task NormalizeLogtoOidcEndpoint_WithDopplerValue_ShouldNotProduceDoubleOidc()
    {
        // This is the exact value stored in Doppler that caused the bug
        const string dopplerValue = "https://32nkyp.logto.app/oidc";

        var result = WebAuthenticationExtensions.NormalizeLogtoOidcEndpoint(dopplerValue);

        result.Should().NotContain("/oidc/oidc", because: "double /oidc suffix breaks OIDC discovery and causes login failure");
        await Assert.That(result).IsEqualTo("https://32nkyp.logto.app/oidc");
    }
}
