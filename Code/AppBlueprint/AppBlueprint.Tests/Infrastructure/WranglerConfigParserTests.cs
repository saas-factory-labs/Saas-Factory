using AppBlueprint.Infrastructure.Core.Providers.Cloudflare.Wrangler;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class WranglerConfigParserTests
{
    private const string SampleToml = """
    name = "dating-web"
    account_id = "acc_123"
    compatibility_date = "2024-09-23"
    compatibility_flags = ["nodejs_compat"]

    [vars]
    API_BASE = "https://api.dating.app"

    [[r2_buckets]]
    binding = "PHOTOS"
    bucket_name = "dating-photos"

    [[hyperdrive]]
    binding = "DB"
    id = "hd_abc"
    """;

    [Test]
    public async Task Parse_ExtractsAllBridgedFields()
    {
        WranglerConfig cfg = WranglerConfigParser.Parse(SampleToml);

        cfg.Name.Should().Be("dating-web");
        cfg.AccountId.Should().Be("acc_123");
        cfg.CompatibilityDate.Should().Be("2024-09-23");
        cfg.CompatibilityFlags.Should().ContainSingle().Which.Should().Be("nodejs_compat");
        cfg.R2Buckets.Should().ContainSingle();
        cfg.R2Buckets[0].Binding.Should().Be("PHOTOS");
        cfg.R2Buckets[0].BucketName.Should().Be("dating-photos");
        cfg.HyperdriveBindings.Should().ContainSingle();
        cfg.HyperdriveBindings[0].Id.Should().Be("hd_abc");
        cfg.Vars.Should().ContainKey("API_BASE");

        await Assert.That(cfg.Vars["API_BASE"]).IsEqualTo("https://api.dating.app");
    }

    [Test]
    public void Parse_MalformedToml_ThrowsInvalidOperationException()
    {
        Action act = () => WranglerConfigParser.Parse("name = ");

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void TryLoadFromDirectory_NoFile_ReturnsNull()
    {
        string emptyDir = Directory.CreateTempSubdirectory().FullName;

        WranglerConfig? result = WranglerConfigParser.TryLoadFromDirectory(emptyDir);

        result.Should().BeNull();
    }
}
