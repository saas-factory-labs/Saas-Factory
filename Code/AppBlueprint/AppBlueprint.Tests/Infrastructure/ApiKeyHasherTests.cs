using AppBlueprint.Infrastructure.Authentication;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class ApiKeyHasherTests
{
    [Test]
    public async Task GenerateRawKey_ShouldNotReturnHardcodedPlaceholder()
    {
        string key = ApiKeyHasher.GenerateRawKey();

        // Regression guard for the "adad" hardcoded SecretRef vulnerability.
        key.Should().NotBe("adad");
        await Assert.That(key).IsNotNull();
    }

    [Test]
    public async Task GenerateRawKey_ShouldHaveExpectedPrefix()
    {
        string key = ApiKeyHasher.GenerateRawKey();

        await Assert.That(key.StartsWith(ApiKeyHasher.KeyPrefix, StringComparison.Ordinal)).IsTrue();
    }

    [Test]
    public async Task GenerateRawKey_ShouldProduceHighEntropyValue()
    {
        string key = ApiKeyHasher.GenerateRawKey();

        // 32 random bytes base64url-encoded is ~43 chars + prefix.
        await Assert.That(key.Length).IsGreaterThanOrEqualTo(40);
    }

    [Test]
    public async Task GenerateRawKey_ShouldBeUniqueAcrossCalls()
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < 1000; i++)
        {
            keys.Add(ApiKeyHasher.GenerateRawKey());
        }

        // No collisions expected from a CSPRNG-backed generator.
        await Assert.That(keys.Count).IsEqualTo(1000);
    }

    [Test]
    public async Task ComputeSha256Hex_ShouldBeDeterministic()
    {
        const string input = "abk_some-known-value";

        string first = ApiKeyHasher.ComputeSha256Hex(input);
        string second = ApiKeyHasher.ComputeSha256Hex(input);

        await Assert.That(first).IsEqualTo(second);
    }

    [Test]
    public async Task ComputeSha256Hex_ShouldReturnLowercase64CharHex()
    {
        string hash = ApiKeyHasher.ComputeSha256Hex("abk_value");

        hash.Should().MatchRegex("^[0-9a-f]{64}$");
        await Assert.That(hash.Length).IsEqualTo(64);
    }

    [Test]
    public async Task ComputeSha256Hex_DifferentInputs_ShouldProduceDifferentHashes()
    {
        string a = ApiKeyHasher.ComputeSha256Hex("abk_one");
        string b = ApiKeyHasher.ComputeSha256Hex("abk_two");

        await Assert.That(a).IsNotEqualTo(b);
    }
}
