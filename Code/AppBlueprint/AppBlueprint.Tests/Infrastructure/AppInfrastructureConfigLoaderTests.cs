using AppBlueprint.Infrastructure.Core.Configuration;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class AppInfrastructureConfigLoaderTests
{
    [Test]
    public async Task Parse_ValidConfig_ReturnsTypedConfig()
    {
        const string json = """
        {
          "appId": "dating",
          "provider": "Cloudflare",
          "compute": [ { "name": "web", "image": "registry/web:1" } ],
          "database": { "useHyperdrive": true },
          "storage": { "buckets": ["photos"], "enableImages": true }
        }
        """;

        AppInfrastructureConfig config = AppInfrastructureConfigLoader.Parse(json);

        config.AppId.Should().Be("dating");
        config.Provider.Should().Be(CloudProvider.Cloudflare);
        config.Compute.Should().ContainSingle();
        config.Compute[0].Image.Should().Be("registry/web:1");
        config.Database!.UseHyperdrive.Should().BeTrue();
        config.Storage!.Buckets.Should().ContainSingle().Which.Should().Be("photos");
        config.Storage.EnableImages.Should().BeTrue();

        await Assert.That(config.AppId).IsEqualTo("dating");
    }

    [Test]
    public async Task Parse_IsCaseInsensitive_AndToleratesCommentsAndTrailingCommas()
    {
        const string json = """
        {
          // a comment
          "APPID": "x",
          "PROVIDER": "Cloudflare",
          "COMPUTE": [ { "NAME": "api", "IMAGE": "i:1" }, ]
        }
        """;

        AppInfrastructureConfig config = AppInfrastructureConfigLoader.Parse(json);

        await Assert.That(config.AppId).IsEqualTo("x");
    }

    [Test]
    [Arguments("""{ "appId":"x", "compute":[{"name":"web","image":"i"}] }""")]                       // missing provider
    [Arguments("""{ "appId":"x", "provider":"AWS", "compute":[{"name":"web","image":"i"}] }""")]      // invalid provider
    [Arguments("""{ "appId":"x", "provider":"Cloudflare" }""")]                                       // no compute
    [Arguments("""{ "appId":"x", "provider":"Cloudflare", "compute":[{"name":"web"}] }""")]           // compute missing image
    public void Parse_InvalidConfig_ThrowsInvalidOperationException(string json)
    {
        Action act = () => AppInfrastructureConfigLoader.Parse(json);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void LoadFromFile_MissingFile_ThrowsFileNotFound()
    {
        string missing = Path.Combine(Path.GetTempPath(), $"nonexistent-{Guid.NewGuid():N}.json");

        Action act = () => AppInfrastructureConfigLoader.LoadFromFile(missing);

        act.Should().Throw<FileNotFoundException>();
    }
}
